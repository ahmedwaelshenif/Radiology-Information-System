using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IMedicalRecordService
    {
        public Task CreateAsync(MedicalRecordRequestDto medicalRecordRequestDto, int recordedByUserId);
        public Task<PagedResponse<MedicalRecordResponse>> SearchAsync(MedicalRecordSearchDto searchDto);
        Task<PagedResponse<MedicalRecordResponse>>GetMyRecordsAsync(int userId, int pageNumber, int pageSize);
        Task<PagedResponse<MedicalRecordResponse>>SearchMyRecordsAsync(int userId, MedicalRecordSearchDto searchDto);


    }
}
