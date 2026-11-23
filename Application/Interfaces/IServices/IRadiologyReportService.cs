
using Application.Common;
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;

namespace Application.Interfaces.IServices
{
    public interface IRadiologyReportService
    {
        Task<PagedResult<RadiologyReportResponseDto>> GetAllAsync(int pageNumber, int pageSize);
        Task CreateAsync(
            CreateRadiologyReportDto dto, int createdByUserId);

        Task<RadiologyReportResponseDto> GetByIdAsync(int reportId);

        Task<byte[]> GetPdfAsync(int reportId);

        Task<PagedResult<RadiologyReportResponseDto>> SearchAsync(
            RadiologyReportSearchDto filter);

        Task UpdateAsync(
            int reportId, UpdateRadiologyReportDto dto);

        Task DeleteAsync(int reportId);

        Task<PagedResult<RadiologyReportResponseDto>> SearchMyReportsAsync(
            RadiologyReportSearchDto filter, int userId);

        Task<PagedResult<RadiologyReportResponseDto>> GetAllMyReportsAsync(int pageNumber, int pageSize, int userId);

        Task<byte[]> GetMyPdfAsync(int reportId, int userId);
    }
}
