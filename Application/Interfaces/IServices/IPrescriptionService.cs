using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IPrescriptionService
    {
        Task<bool> CreateAsync(PrescriptionRequestDto prescriptionDto,int createdById);
        Task<PrescriptionResponseDto?> GetByIdAsync(int id);
        Task<PagedResponse<PrescriptionResponseDto>> SearchAsync(PrescriptionSearchDto searchDto);
        Task<PagedResponse<PrescriptionResponseDto>> GetByTreatmentPlanIdAsync(int id);
        Task<PagedResponse<PrescriptionResponseDto>> GetByPatientIdAsync(int id);
        Task<bool> CancelPrescriptionAsync(int id);
        Task<PrescriptionUpdateResult> UpdateAsync(int id, PrescriptionRequestDto prescriptionDto, int createdById);


    }
}
