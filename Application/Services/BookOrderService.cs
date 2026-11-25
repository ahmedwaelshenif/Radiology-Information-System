using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Specifications;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class BookOrderService : IBookOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CancelMyBookOrderAsync(int id, int userId)
        {
            int patientId=await GetPatientId(userId);

            var bookOrder= await _unitOfWork.Repository<BookOrder>().FirstOrDefaultAsync(b => b.PatientId == patientId && b.Id==id);
            if (bookOrder == null)
                throw new KeyNotFoundException("Order request Doesn't Exist.");

            if (bookOrder.Status != BookOrderStatus.Pending)
                throw new InvalidOperationException("Order request has already been proccessed.");

            bookOrder.Status = BookOrderStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task<int> CountByStatusAsync(BookOrderStatus status)
        {
            return await _unitOfWork.Repository<BookOrder>().CountAsync(b => b.Status == status);
        }

        public async Task<int> CountMyBookOrdersByStatusAsync(BookOrderStatus status,int userId)
        {
           int patientId=await GetPatientId(userId);
            return await _unitOfWork.Repository<BookOrder>().CountAsync(b => b.Status == status && b.PatientId== patientId);
        }

        public async Task CreateAsync(CreateBookOrderDto dto,int userId)
        {

            if (dto.ScheduledAt.Date < DateTime.Today)
                throw new InvalidOperationException(
                    "Scheduled date cannot be in the past.");

            int patientId = await GetPatientId(userId);


            var bookOrder = ToEntity(dto, patientId);
            await _unitOfWork.Repository<BookOrder>().AddAsync(bookOrder);
            await _unitOfWork.SaveChangesAsync();

            
        }

        public  async Task<PagedResponse<BookOrderResponseDto>> GetAllAsync(PagedRequestDto pagination)
        {
            return await SearchAsync(new BookOrderSearchDto { PageNumber = pagination.PageNumber, PageSize = pagination.PageSize });
            
        }

        public async Task<PagedResponse<BookOrderResponseDto>> GetAllMyBookOrdersAsync(int userId, PagedRequestDto pagination)
        {
            int patientId = await GetPatientId(userId);
            return await SearchAsync(new BookOrderSearchDto { PageNumber = pagination.PageNumber,
                                                              PageSize = pagination.PageSize,
                                                              PatientId=patientId
            });
        }

        public async Task<BookOrderResponseDto> GetById(int id)
        {
            var specification = new BookOrderSpecification(new BookOrderSearchDto { Id = id });
            var bookOrder = await _unitOfWork.Repository<BookOrder>().GetEntityWithSpecAsync(specification);
   
            if (bookOrder == null)
                throw new KeyNotFoundException("Order Request Doesn't exist");
            return ToDto(bookOrder);
        }

        public async Task<BookOrderResponseDto> GetMyById(int id, int userId)
        {
            int patientId = await GetPatientId(userId);

            var specification = new BookOrderSpecification(
                new BookOrderSearchDto
                {
                    Id = id,
                    PatientId = patientId
                });

            var bookOrder = await _unitOfWork.Repository<BookOrder>()
                .GetEntityWithSpecAsync(specification);

            if (bookOrder == null)
                throw new KeyNotFoundException("Order request doesn't exist.");

            return ToDto(bookOrder);
        }

        public async Task<PagedResponse<BookOrderResponseDto>> SearchAsync(BookOrderSearchDto dto)
        {
            var specification = new BookOrderSpecification(dto);
            var bookOrders= await _unitOfWork.Repository<BookOrder>().ListAsync(specification);
            int totalCount= await _unitOfWork.Repository<BookOrder>().CountAsync(specification);
            return new PagedResponse<BookOrderResponseDto>
            {
                Items = bookOrders.Select(ToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
            };
        }

        public async Task<PagedResponse<BookOrderResponseDto>> SearchMyBookOrdersAsync(BookOrderSearchDto dto, int userId)
        {
            int patientId = await GetPatientId(userId);
            dto.PatientId = patientId;
            dto.PatientName = null;
            return await SearchAsync(dto);
        }

        public async Task RejectAsync(int id)
        {
            var bookOrder=await _unitOfWork.Repository<BookOrder>().GetByIdAsync(id);
            if (bookOrder == null)
                throw new KeyNotFoundException("Order Reequest Doesn't Exist");
            if (bookOrder.Status != BookOrderStatus.Pending)
                throw new InvalidOperationException("Order request has already been proccessed.");

           
            bookOrder.Status = BookOrderStatus.Rejected;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AcceptAsync(int id,int orderedByUserId)
        {
            var bookOrder = await _unitOfWork.Repository<BookOrder>().GetByIdAsync(id);
            if (bookOrder == null)
                throw new KeyNotFoundException("Order Reequest Doesn't Exist");
            if (bookOrder.Status != BookOrderStatus.Pending)
                throw new InvalidOperationException("Order request has already been proccessed.");


            var patientExists = await _unitOfWork.Repository<Patient>()
                .AnyAsync(p => p.Id == bookOrder.PatientId);
            if (!patientExists)
                throw new KeyNotFoundException($"Patient {bookOrder.PatientId} was not found.");


            var userIsActive = await _unitOfWork.Repository<User>()
                .AnyAsync(u => u.Id == orderedByUserId && u.IsActive);
            if (!userIsActive)
                throw new KeyNotFoundException($"Active user {orderedByUserId} was not found.");


            if (bookOrder.ScheduledAt <= DateTime.UtcNow)
                throw new ArgumentException("ScheduledAt must be in the future.");

            var order = new RadiologyOrder
            {
                PatientId = bookOrder.PatientId,
                OrderedByUserId = orderedByUserId,
                Modality = bookOrder.Modality,
                BodyPart = bookOrder.BodyPart.Trim(),
                Status = OrderStatus.Scheduled,
                AccessionNumber = await GenerateAccessionNumberAsync(),
                OrderedAt = DateTime.UtcNow,
                ScheduledAt = bookOrder.ScheduledAt
            };



            var availableUids = (await _unitOfWork.Repository<StudyUID>().GetAllAsync(s => s.Modality == bookOrder.Modality))?.ToList() ?? new List<StudyUID>();

            if (availableUids.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No StudyInstanceUID available for modality {bookOrder.Modality}");
            }

            var selectedUid = availableUids[
                Random.Shared.Next(availableUids.Count)
            ];

            var study = new RadiologyStudy
            {
                Status = StudyStatus.Pending,
                Order = order,
                StudyInstanceUID = selectedUid.StudyInstanceUID

            };

            Price? mod = await _unitOfWork.Repository<Price>().FirstOrDefaultAsync(p => p.Name == bookOrder.Modality);
            if (mod == null)
            {
                throw new ArgumentException("this modality can not be found");
            }

            var invoice = new Invoice
            {
                Amount = mod.Amount,
                Order = order,
                PatientId = bookOrder.PatientId,
                CreatedAt = DateTime.UtcNow,
                PaymentMethod = bookOrder.PaymentMethod,
            };
            await _unitOfWork.Repository<RadiologyOrder>().AddAsync(order);
            await _unitOfWork.Repository<RadiologyStudy>().AddAsync(study);
            await _unitOfWork.Repository<Invoice>().AddAsync(invoice);
            bookOrder.Status = BookOrderStatus.Accepted;

            await _unitOfWork.SaveChangesAsync();
        }





        //Helper Methods

        private BookOrder ToEntity(CreateBookOrderDto dto,int patientId)
        {
            return new BookOrder
            {
                ScheduledAt = dto.ScheduledAt,
                Status = BookOrderStatus.Pending,
                BodyPart = dto.BodyPart,
                Modality = dto.Modality,
                OrderedAt = DateTime.UtcNow,
                PatientId = patientId,
                PaymentMethod= dto.PaymentMethod,
            };
        }
        private BookOrderResponseDto ToDto (BookOrder bookOrder)
        {
            return new BookOrderResponseDto
            {
                ScheduledAt=bookOrder.ScheduledAt,
                Status = bookOrder.Status,
                BodyPart=bookOrder.BodyPart,
                Id = bookOrder.Id,
                Modality=bookOrder.Modality,
                OrderedAt= bookOrder.OrderedAt,
                PatientId=bookOrder.PatientId,
                PatientName=bookOrder.Patient.User.FullName,
                PaymentMethod=bookOrder.PaymentMethod,
            };
        }

        private async Task<int> GetPatientId(int userId)
        {
            var patient = await _unitOfWork.Repository<Patient>().FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
                throw new KeyNotFoundException("Patient Doesn't Exist.");
            return patient.Id;
        }

        private async Task<string> GenerateAccessionNumberAsync()
        {
            var repo = _unitOfWork.Repository<RadiologyOrder>();
            string accession;

            do
            {
                accession = $"ACC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
            }
            while (await repo.AnyAsync(o => o.AccessionNumber == accession));

            return accession;
        }

    }
}
