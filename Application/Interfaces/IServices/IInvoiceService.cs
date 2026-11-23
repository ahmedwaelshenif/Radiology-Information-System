using Application.Common;
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.IServices
{
    public interface IInvoiceService
    {
        
        Task<InvoiceResponseDto> GetByIdAsync(int invoiceId);

        Task<InvoiceResponseDto> GetByOrderIdAsync(int orderId);

        Task<PagedResult<InvoiceResponseDto>> GetPagedAsync(
            int pageNumber, int pageSize);

        Task<PagedResult<InvoiceResponseDto>> GetByPatientAsync(
            int patientId, int pageNumber, int pageSize);

        Task<PagedResult<InvoiceResponseDto>> SearchAsync(
            InvoiceSearchDto filter);

        Task<PagedResult<InvoiceResponseDto>> SearchMyInvoicesAsync(
           InvoiceSearchDto filter, int userId);

        Task<PagedResult<InvoiceResponseDto>> GetAllMyInvoicesAsync(
         int userId, int pageSize = 10, int pageNumber = 1);


        Task<decimal> CountAmountWithin(DateTime start, DateTime end);

     

       

    }
}
