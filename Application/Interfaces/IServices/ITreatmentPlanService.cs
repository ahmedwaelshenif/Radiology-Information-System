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
        Task<TreatmentPlanResponseDto?> GetMyTreatmentPlanByIdAsync(int id,int userId);
        Task<PagedResponse<TreatmentPlanResponseDto>> SearchAsync(TreatmentPlanSearchDto searchDto);
        Task<PagedResponse<TreatmentPlanResponseDto>> SearchMyTreatmentPlansAsync(TreatmentPlanSearchDto searchDto,int userId);
        Task<PagedResponse<TreatmentPlanResponseDto>> GetAllMyTreatmentPlansAsync(int pageNumber,int pageSize,int userId);
        Task<PagedResponse<TreatmentPlanResponseDto>> GetByPatientIdAsync(int id);
        public Task UpdateTreatmentPlanStatusAsync(int id, bool status);
    }
}
