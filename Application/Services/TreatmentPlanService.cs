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

        public async Task UpdateTreatmentPlanStatusAsync(int id,bool status)
        {
            var treatmentPlan=await _unitOfWork.Repository<TreatmentPlan>().GetByIdAsync(id);
            if (treatmentPlan == null)
                throw new KeyNotFoundException("Treatment Plan not found");
            if (treatmentPlan.Status != TreatmentPlanStatus.Active)
                throw new InvalidOperationException("status can't be updated");

            if(status)
                treatmentPlan.Status = TreatmentPlanStatus.Completed;
            else treatmentPlan.Status = TreatmentPlanStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync();
            
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


        public async Task<TreatmentPlanResponseDto?> GetMyTreatmentPlanByIdAsync(int id, int userId)
        {
            var res= await _unitOfWork.Repository<TreatmentPlan>().FirstOrDefaultAsync(t => t.Id == id && t.Patient.UserId == userId);
            if (res == null)
                throw new KeyNotFoundException("TreatmentPlan not found");
            return ToDto(res);
        }

        public async Task<PagedResponse<TreatmentPlanResponseDto>> SearchMyTreatmentPlansAsync(TreatmentPlanSearchDto searchDto, int userId)
        {
            searchDto.UserId = userId;
            searchDto.PatientId = null;
            searchDto.PatientName = null;
            return await SearchAsync(searchDto);
        }

        public async Task<PagedResponse<TreatmentPlanResponseDto>> GetAllMyTreatmentPlansAsync(int pageNumber, int pageSize, int userId)
        {
            return await SearchAsync(new TreatmentPlanSearchDto
            {
                UserId=userId,
                PageNumber=pageNumber,
                PageSize=pageSize
            });
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
