using Application.Common;
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Specifications;
using Domain.Entities;
using Domain.Enums;
using Domain.Specifications;

namespace Application.Services
{
    public class RadiologyOrderService : IRadiologyOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RadiologyOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


       public async Task CreateAsync(CreateRadiologyOrderDto dto, int orderedByUserId)
        {
            var patientExists = await _unitOfWork.Repository<Patient>()
                .AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists)
                throw new KeyNotFoundException($"Patient {dto.PatientId} was not found.");

            var userIsActive = await _unitOfWork.Repository<User>()
                .AnyAsync(u => u.Id == orderedByUserId && u.IsActive);
            if (!userIsActive)
                throw new KeyNotFoundException($"Active user {orderedByUserId} was not found.");

            if ( dto.ScheduledAt <= DateTime.UtcNow)
                throw new ArgumentException("ScheduledAt must be in the future.");

            var order = new RadiologyOrder
            {
                PatientId = dto.PatientId,
                OrderedByUserId = orderedByUserId,
                Modality = dto.Modality,
                BodyPart = dto.BodyPart.Trim(),
                Status = OrderStatus.Scheduled,
                AccessionNumber = await GenerateAccessionNumberAsync(),
                OrderedAt = DateTime.UtcNow,
                ScheduledAt = dto.ScheduledAt
            };

           

            var availableUids = (await _unitOfWork.Repository<StudyUID>().GetAllAsync(s => s.Modality == dto.Modality))?.ToList()?? new List<StudyUID>();

            if (availableUids.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No StudyInstanceUID available for modality {dto.Modality}");
            }

            var selectedUid = availableUids[
                Random.Shared.Next(availableUids.Count)
            ];

            var study = new RadiologyStudy {
                Status=StudyStatus.Pending,
                Order=order,
                StudyInstanceUID = selectedUid.StudyInstanceUID

            };
            await _unitOfWork.Repository<RadiologyOrder>().AddAsync(order);
            await _unitOfWork.Repository<RadiologyStudy>().AddAsync(study);
            await _unitOfWork.SaveChangesAsync();

        }

        // ---------- Read ----------
        public async Task<RadiologyOrderResponseDto> GetByIdAsync(int orderId)
        {
            RadiologyOrderSearchDto filter = new();
            filter.OrderId = orderId;
            var orders = await _unitOfWork.Repository<RadiologyOrder>()
                .ListAsync(new RadiologyOrderSearchSpecification(filter));

            var order = orders.FirstOrDefault()
                ?? throw new KeyNotFoundException($"Radiology order {orderId} was not found.");

            return ToDto(order);
        }

        public async Task<RadiologyOrderResponseDto> GetByAccessionNumberAsync(string accessionNumber)
        {
            RadiologyOrderSearchDto filter = new();
            filter.AccessionNumber = accessionNumber;
            var order = await _unitOfWork.Repository<RadiologyOrder>()
                .GetEntityWithSpecAsync(new RadiologyOrderSearchSpecification(filter));


            if(order==null)
                throw new KeyNotFoundException($"Radiology order with accession number '{accessionNumber}' was not found.");

            return ToDto(order);
        }

        public async Task<PagedResult<RadiologyOrderResponseDto>> GetPagedAsync(
            int pageNumber, int pageSize)
        {

            var spec = new RadiologyOrderWithIncludeSpecification(pageNumber, pageSize);
            return await ListPagedAsync(spec);
        }

        public async Task<PagedResult<RadiologyOrderResponseDto>> GetByPatientAsync(
            int patientId, int pageNumber, int pageSize)
        {
            RadiologyOrderSearchDto filter = new();
            filter.PatientId = patientId;
            filter.PageNumber = pageNumber;
            filter.PageSize = pageSize;
            var spec = new RadiologyOrderSearchSpecification(filter);

            return await ListPagedAsync(spec);
        }

        public async Task<PagedResult<RadiologyOrderResponseDto>> SearchAsync(
            RadiologyOrderSearchDto filter)
        {
            var spec = new RadiologyOrderSearchSpecification(filter);
            return await ListPagedAsync(spec);
        }



        public async Task CancelAsync(
            int orderId)
        {
            var order = await _unitOfWork.Repository<RadiologyOrder>().GetEntityWithSpecAsync(new RadiologyOrderSearchSpecification(
                new RadiologyOrderSearchDto { OrderId = orderId }));
            if (order == null)
                throw new KeyNotFoundException("Order doesn't Exist");

            
            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException(
                    $"Order is already {order.Status} and cannot be changed.");

            order.Status = OrderStatus.Cancelled;

            order.Study.Status = StudyStatus.Cancelled;

            await SaveAsync(order);
          
        }




        public async Task<PagedResult<RadiologyOrderResponseDto>> GetAllMyOrdersPagedAsync(int userId,int pageNumber, int pageSize)
        {
            return await SearchAsync(new RadiologyOrderSearchDto { UserId= userId , PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        public async Task<PagedResult<RadiologyOrderResponseDto>> SearchMyOrdersAsync(int userId,RadiologyOrderSearchDto filter)
        {
            filter.PatientId = null;
            filter.UserId = userId;
            return await SearchAsync(filter);
        }

        // ---------- Helpers ----------
        private async Task<RadiologyOrder> GetTrackedOrderAsync(int orderId)
        {
            return await _unitOfWork.Repository<RadiologyOrder>().GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException($"Radiology order {orderId} was not found.");
        }

        private async Task SaveAsync(RadiologyOrder order)
        {
            _unitOfWork.Repository<RadiologyOrder>().Update(order);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<PagedResult<RadiologyOrderResponseDto>> ListPagedAsync(
            ISpecification<RadiologyOrder> spec)
        {
            var repo = _unitOfWork.Repository<RadiologyOrder>();

            var items = await repo.ListAsync(spec);
            var total = await repo.CountAsync(spec);

            return new PagedResult<RadiologyOrderResponseDto>
            {
                Items = items.Select(ToDto).ToList(),
                TotalCount = total
            };
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

        private static RadiologyOrderResponseDto ToDto(RadiologyOrder o) => new()
        {
            OrderId = o.Id,
            PatientId = o.PatientId,
            PatientName = o.Patient?.User?.FullName ?? string.Empty,
            OrderedByUserId = o.OrderedByUserId,
            OrderedByName = o.OrderedByUser?.FullName ?? string.Empty,
            Modality = o.Modality.ToString(),
            BodyPart = o.BodyPart,
            Status = o.Status,
            AccessionNumber = o.AccessionNumber,
            OrderedAt = o.OrderedAt,
            ScheduledAt = o.ScheduledAt,
            studyId=o.Study.Id,
            StudyStatus = o.Study.Status,
        };

  
    }
}
