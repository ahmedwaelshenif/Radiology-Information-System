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

        // ---------- Create ----------
        public async Task CreateAsync(CreateInvoiceDto dto)
        {
            var order = await _unitOfWork.Repository<RadiologyOrder>().GetByIdAsync(dto.OrderId)
                ?? throw new KeyNotFoundException($"Radiology order {dto.OrderId} was not found.");

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot create an invoice for a cancelled order.");

            // one invoice per order
            var alreadyBilled = await _unitOfWork.Repository<Invoice>()
                .AnyAsync(i => i.OrderId == dto.OrderId);
            if (alreadyBilled)
                throw new InvalidOperationException($"Order {dto.OrderId} already has an invoice.");

            var invoice = new Invoice
            {
                PatientId = order.PatientId,
                OrderId = order.Id,
                ServiceName = dto.ServiceName.Trim(),
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = InvoiceStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Invoice>().AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
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

        public async Task<InvoiceResponseDto> UpdateStatusAsync(
            int invoiceId, InvoiceStatus status)
        {
            var invoice = await GetTrackedInvoiceAsync(invoiceId);

            // Cancelled and Refunded are final states
            if (invoice.Status == InvoiceStatus.Cancelled || invoice.Status == InvoiceStatus.Refunded)
                throw new InvalidOperationException(
                    $"Invoice is already {invoice.Status} and cannot be changed.");

            // Allowed transitions: Pending -> Paid | Cancelled, Paid -> Refunded
            var allowed =
                (invoice.Status == InvoiceStatus.Pending &&
                    (status == InvoiceStatus.Paid || status == InvoiceStatus.Cancelled)) ||
                (invoice.Status == InvoiceStatus.Paid && status == InvoiceStatus.Refunded);

            if (!allowed)
                throw new InvalidOperationException(
                    $"Cannot change invoice status from {invoice.Status} to {status}.");

            if (status == InvoiceStatus.Paid)
                invoice.PaidAt = DateTime.UtcNow;

            invoice.Status = status;

            await SaveAsync(invoice);
            return await GetByIdAsync(invoiceId);
        }

        public Task<InvoiceResponseDto> CancelAsync(int invoiceId)
            => UpdateStatusAsync(invoiceId, InvoiceStatus.Cancelled);



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



        //Helper Method
        private async Task<Invoice> GetTrackedInvoiceAsync(int invoiceId)
        {
            return await _unitOfWork.Repository<Invoice>().GetByIdAsync(invoiceId)
                ?? throw new KeyNotFoundException($"Invoice {invoiceId} was not found.");
        }

        private async Task SaveAsync(Invoice invoice)
        {
            _unitOfWork.Repository<Invoice>().Update(invoice);
            await _unitOfWork.SaveChangesAsync();
        }

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
            ServiceName = i.ServiceName,
            Amount = i.Amount,
            PaymentMethod = i.PaymentMethod,
            Status = i.Status,
            CreatedAt = i.CreatedAt,
            PaidAt = i.PaidAt
        };


    }
}
