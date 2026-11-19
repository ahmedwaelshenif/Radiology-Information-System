using Application.Common;
using Application.DTOs;
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Enums;

namespace Application.Interfaces.IServices
{
    public interface IPatientService
    {
        Task<RegisterResult> Register(RegisterPatientDto patientDto);
        Task<PagedResponse<PatientResponseDto>> GetAllAsync(int pageNumber,int pageSize);
        Task<PagedResponse<PatientResponseDto>> SearchAsync(PatientSearchDto searchDto);


    }
}
