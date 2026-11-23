using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Enums;

namespace Application.Interfaces.IServices
{
    public interface IPrescriptionService
    {
        Task<bool> CreateAsync(PrescriptionRequestDto prescriptionDto,int createdById);
        Task<PrescriptionResponseDto?> GetByIdAsync(int id);
        Task<PagedResponse<PrescriptionResponseDto>> SearchAsync(PrescriptionSearchDto searchDto);
        Task<PagedResponse<PrescriptionResponseDto>> SearchMyPrescriptionsAsync(PrescriptionSearchDto searchDto,int userId);
        Task<PagedResponse<PrescriptionResponseDto>> GetAllMyPrescriptionsAsync(int pageNumber,int pageSize,int userId);
        Task<bool> CancelPrescriptionAsync(int id);
        Task<PrescriptionUpdateResult> UpdateAsync(int id, PrescriptionRequestDto prescriptionDto, int createdById);


    }
}
