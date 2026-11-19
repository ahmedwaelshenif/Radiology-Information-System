using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
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
        Task<RadiologyStudyResponseDto> GetByIdAsync(int id);
    }
}
