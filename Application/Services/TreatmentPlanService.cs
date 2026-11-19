using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Specifications;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class TreatmentPlanService : ITreatmentPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TreatmentPlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task CreateAsync(TreatmentPlanRequestDto treatmentPlanDto, int createdById)
        {
            var treatmentPlan = new TreatmentPlan
            {
                CreatedByUserId = createdById,
                PatientId = treatmentPlanDto.PatientId,
                PlanTitle = treatmentPlanDto.PlanTitle,
                PlanDetails = treatmentPlanDto.PlanDetails,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Status=TreatmentPlanStatus.Active,               
            };

            await _unitOfWork.Repository<TreatmentPlan>().AddAsync(treatmentPlan);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> CancelTreatmentPlanAsync(int id)
        {
            var treatmentPlan=await _unitOfWork.Repository<TreatmentPlan>().GetByIdAsync(id);
            if (treatmentPlan == null)
                return false;
            treatmentPlan.Status = TreatmentPlanStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<TreatmentPlanResponseDto?> GetByIdAsync(int id)
        {
            var treatmentPlan = await _unitOfWork.Repository<TreatmentPlan>().GetEntityWithSpecAsync(new TreatmentPlanSearchSpecification(new TreatmentPlanSearchDto { Id = id }));
            if (treatmentPlan == null)
                return null;

            return ToDto(treatmentPlan);
        }

        public async Task<PagedResponse<TreatmentPlanResponseDto>> GetByPatientIdAsync(int id)
        {
            return await SearchAsync(new TreatmentPlanSearchDto { PatientId = id });
        }

        public async Task<PagedResponse<TreatmentPlanResponseDto>> SearchAsync(TreatmentPlanSearchDto searchDto)
        {
            var result = await GetListWithSpecAsync(searchDto);

            return new PagedResponse<TreatmentPlanResponseDto>
            {
                Items = result.Items.Select(ToDto).ToList(),
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                TotalCount = result.TotalCount,

            };
        }
 

        //Helper Methods

        private async Task<PagedResponse<TreatmentPlan>> GetListWithSpecAsync(TreatmentPlanSearchDto searchDto)
        {
            var specification = new TreatmentPlanSearchSpecification(searchDto);
            var treatmentPlans = await _unitOfWork.Repository<TreatmentPlan>().ListAsync(specification);
            var totalCount = await _unitOfWork.Repository<TreatmentPlan>().CountAsync(specification);

            return new PagedResponse<TreatmentPlan>
            {
                Items = treatmentPlans,
                TotalCount = totalCount
            };

        }
        private TreatmentPlanResponseDto ToDto(TreatmentPlan treatmentPlan)
        {
            return new TreatmentPlanResponseDto
            {
                Id = treatmentPlan.Id,
                PatientId = treatmentPlan.PatientId,
                PlanDetails = treatmentPlan.PlanDetails,
                PlanTitle = treatmentPlan.PlanTitle,
                PatientName = treatmentPlan.Patient.User.FullName
            };
        }
    }
}
