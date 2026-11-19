using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Specifications;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface ITreatmentPlanService
    {
        Task CreateAsync(TreatmentPlanRequestDto treatmentPlanDto, int createdById);
        Task<TreatmentPlanResponseDto?> GetByIdAsync(int id);
        Task<PagedResponse<TreatmentPlanResponseDto>> SearchAsync(TreatmentPlanSearchDto searchDto);
        Task<PagedResponse<TreatmentPlanResponseDto>> GetByPatientIdAsync(int id);
        Task<bool> CancelTreatmentPlanAsync(int id);
    }
}
