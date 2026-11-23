using Application.Common;
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.IServices
{
    public interface IRadiologyOrderService
    {
        Task CreateAsync(
            CreateRadiologyOrderDto dto, int orderedByUserId);

        Task<RadiologyOrderResponseDto> GetByIdAsync(int orderId);

        Task<RadiologyOrderResponseDto> GetByAccessionNumberAsync(string accessionNumber);

        Task<PagedResult<RadiologyOrderResponseDto>> GetPagedAsync(
            int pageNumber, int pageSize);

        Task<PagedResult<RadiologyOrderResponseDto>> GetAllMyOrdersPagedAsync(
           int userId, int pageNumber, int pageSize);

        Task<PagedResult<RadiologyOrderResponseDto>> GetByPatientAsync(
            int patientId, int pageNumber, int pageSize);

        Task<PagedResult<RadiologyOrderResponseDto>> SearchAsync(
            RadiologyOrderSearchDto filter);

        Task<PagedResult<RadiologyOrderResponseDto>> SearchMyOrdersAsync(
           int UserId,RadiologyOrderSearchDto filter);
     
        Task CancelAsync(int orderId);

        Task<int> CountAllOrdersByStatus(OrderStatus ?status=null);



    }
}
