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
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InvoiceResponseDto> GetByIdAsync(int invoiceId)
        {
            InvoiceSearchDto filter = new();
            filter.InvoiceId = invoiceId;
            var invoices = await _unitOfWork.Repository<Invoice>()
                .ListAsync(new InvoiceSearchSpecification(filter));

            var invoice = invoices.FirstOrDefault()
                ?? throw new KeyNotFoundException($"Invoice {invoiceId} was not found.");

            return ToDto(invoice);
        }

        public async Task<InvoiceResponseDto> GetByOrderIdAsync(int orderId)
        {
            InvoiceSearchDto filter = new();
            filter.OrderId = orderId;
            var invoices = await _unitOfWork.Repository<Invoice>()
                .ListAsync(new InvoiceSearchSpecification(filter));

            var invoice = invoices.FirstOrDefault()
                ?? throw new KeyNotFoundException($"No invoice was found for order {orderId}.");

            return ToDto(invoice);
        }

        public async Task<PagedResult<InvoiceResponseDto>> GetPagedAsync(
            int pageNumber, int pageSize)
        {
            var spec = new InvoiceWithIncludeSpecification(pageNumber, pageSize);
            return await ListPagedAsync(spec);
        }

        public async Task<PagedResult<InvoiceResponseDto>> GetByPatientAsync(
            int patientId, int pageNumber, int pageSize)
        {
            InvoiceSearchDto filter = new();
            filter.PatientId = patientId;
            filter.PageNumber = pageNumber;
            filter.PageSize = pageSize;
            var spec = new InvoiceSearchSpecification(filter);

            return await ListPagedAsync(spec);
        }

        public async Task<PagedResult<InvoiceResponseDto>> SearchAsync(
            InvoiceSearchDto filter)
        {
            var spec = new InvoiceSearchSpecification(filter);
            return await ListPagedAsync(spec);
        }

       

     

        public async Task<PagedResult<InvoiceResponseDto>> SearchMyInvoicesAsync(InvoiceSearchDto filter, int userId)
        {
            filter.PatientName = null;
            filter.PatientId = null;
            filter.UserId = userId;
            return await SearchAsync(filter);
        }

        public async Task<PagedResult<InvoiceResponseDto>> GetAllMyInvoicesAsync(int userId,int pageSize=10,int pageNumber=1)
        {
           
            return await SearchAsync(new InvoiceSearchDto
            {
                UserId = userId,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }


        public async Task<decimal> CountAmountWithin(DateTime start, DateTime end)
        {
            if (start > end)
            {
                throw new ArgumentException("End date should be greater than start date");
            }
            var invoices = await _unitOfWork.Repository<Invoice>().GetAllAsync(i=>i.CreatedAt>=start && i.CreatedAt<=end);
            return invoices.Sum(i=>i.Amount);
            
        }




        //Helper Method


        private async Task<PagedResult<InvoiceResponseDto>> ListPagedAsync(
            ISpecification<Invoice> spec)
        {
            var repo = _unitOfWork.Repository<Invoice>();

            var items = await repo.ListAsync(spec);
            var total = await repo.CountAsync(spec);

            return new PagedResult<InvoiceResponseDto>
            {
                Items = items.Select(ToDto).ToList(),
                TotalCount = total
            };
        }

       

        private static InvoiceResponseDto ToDto(Invoice i) => new()
        {
            InvoiceId = i.Id,
            PatientId = i.PatientId,
            PatientName = i.Patient?.User?.FullName ?? string.Empty,
            OrderId = i.OrderId,
            AccessionNumber = i.Order?.AccessionNumber ?? string.Empty,
            Amount = i.Amount,
            PaymentMethod = i.PaymentMethod,
            CreatedAt = i.CreatedAt
           
        };

        
    }
}
