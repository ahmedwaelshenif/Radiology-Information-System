using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IMedicalRecordService
    {
        public Task<PagedResponse<MedicalRecordResponse>> SearchAsync(MedicalRecordSearchDto searchDto);
        public Task CreateAsync(MedicalRecordRequestDto medicalRecordRequestDto, int recordedByUserId);

    }
}
