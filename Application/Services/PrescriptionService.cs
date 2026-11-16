using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;


namespace Application.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PrescriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateAsync(PrescriptionRequestDto prescriptionDto, int createdById)
        {
            bool created = await AddPrescriptionAsync(prescriptionDto, createdById);

            if (!created) return false;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<PrescriptionResponseDto?> GetByIdAsync(int id)
        {
            var prescription = await _unitOfWork.Repository<Prescription>().GetEntityWithSpecAsync(new PrescriptionSearchSpecification(new PrescriptionSearchDto { Id = id }));
            if (prescription == null)
                return null;

            return ToDto(prescription);
        }

        public async Task<PagedResponse<PrescriptionResponseDto>> GetByPatientIdAsync(int id)
        {
            return await SearchAsync(new PrescriptionSearchDto { PatientId= id });
        }

        public async Task<PagedResponse<PrescriptionResponseDto>> GetByTreatmentPlanIdAsync(int id)
        {
            return await SearchAsync(new PrescriptionSearchDto { TreatmentPlanId = id });
        }

        public async Task<PagedResponse<PrescriptionResponseDto>> SearchAsync(PrescriptionSearchDto searchDto)
        {
            var result = await GetListWithSpecAsync(searchDto);
            return new PagedResponse<PrescriptionResponseDto>
            {
                Items = result.Items.Select(ToDto).ToList(),
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                TotalCount = result.TotalCount,

            };
        }

        public async Task<bool> CancelPrescriptionAsync (int id)
        {
            var prescription=await _unitOfWork.Repository<Prescription>().GetByIdAsync(id);
            if (prescription == null) return false;
            prescription.Status = PrescriptionStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        public async Task<PrescriptionUpdateResult> UpdateAsync(int id, PrescriptionRequestDto prescriptionDto, int createdById)
        {
            var prescription = await _unitOfWork.Repository<Prescription>().GetByIdAsync(id);
            if (prescription == null) return PrescriptionUpdateResult.PrescriptionNotFound;
            


            bool created = await AddPrescriptionAsync(prescriptionDto, createdById);
            if (!created) return PrescriptionUpdateResult.TreatmentPlanNotFound;
            prescription.Status = PrescriptionStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync();

            return PrescriptionUpdateResult.Success;
        }



        //Helper Methods

        private async Task<PagedResponse<Prescription>> GetListWithSpecAsync(PrescriptionSearchDto searchDto)
        {
            var specification = new PrescriptionSearchSpecification(searchDto);
            var prescriptions = await _unitOfWork.Repository<Prescription>().ListAsync(specification);
            var totalCount = await _unitOfWork.Repository<Prescription>().CountAsync(specification);

            return new PagedResponse<Prescription>
            {
                Items = prescriptions,
                TotalCount = totalCount
            };

        }

        private PrescriptionResponseDto ToDto(Prescription prescription) {
            return new PrescriptionResponseDto
            {
                CreatedBy = prescription.CreatedByUser.FullName,
                Date=prescription.Date,
                Details = prescription.Details,
                TreatmentPlanId = prescription.TreatmentPlanId,
                Id = prescription.Id,
                Status= prescription.Status,
            };
        }

        private async Task<bool> AddPrescriptionAsync(PrescriptionRequestDto dto,int createdById)
        {
            bool exists = await _unitOfWork.Repository<TreatmentPlan>()
                .AnyAsync(t => t.Id == dto.TreatmentPlanId);

            if (!exists) return false;

            var prescription = new Prescription
            {
                CreatedByUserId = createdById,
                TreatmentPlanId = dto.TreatmentPlanId,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Details = dto.Details
            };

            await _unitOfWork.Repository<Prescription>().AddAsync(prescription);

            return true;
        }


    }
}
