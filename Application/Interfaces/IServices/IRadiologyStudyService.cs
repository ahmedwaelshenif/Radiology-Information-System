using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IRadiologyStudyService
    {
        Task UpdateStatusAsync(StudyStatus status,int id);
        Task<PagedResponse<RadiologyStudyResponseDto>> SearchAsync(RadiologyStudySearchDto searchDto);
        Task<PagedResponse<RadiologyStudyResponseDto>> SearchMyStudiesAsync(int userId,RadiologyStudySearchDto searchDto);
        Task<PagedResponse<RadiologyStudyResponseDto>> GetMyStudiesAsync(int pageSize,int pageNumber,int userId);
        Task<RadiologyStudyResponseDto> GetByIdAsync(int id);
        Task<RadiologyStudyResponseDto> GetMyStudyByIdAsync(int id, int userId);

        Task<int> CountByStatus(StudyStatus? status = null);






    }
}
