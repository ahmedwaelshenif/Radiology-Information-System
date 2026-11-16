using Application.Common;
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Domain.Enums;

namespace Application.Interfaces.IServices
{
    public interface IInvoiceService
    {
        Task CreateAsync(CreateInvoiceDto dto);

        Task<InvoiceResponseDto> GetByIdAsync(int invoiceId);

        Task<InvoiceResponseDto> GetByOrderIdAsync(int orderId);

        Task<PagedResult<InvoiceResponseDto>> GetPagedAsync(
            int pageNumber, int pageSize);

        Task<PagedResult<InvoiceResponseDto>> GetByPatientAsync(
            int patientId, int pageNumber, int pageSize);

        Task<PagedResult<InvoiceResponseDto>> SearchAsync(
            InvoiceSearchDto filter);

        Task<InvoiceResponseDto> UpdateStatusAsync(
            int invoiceId, InvoiceStatus status);

        Task<InvoiceResponseDto> CancelAsync(int invoiceId);
    }
}
