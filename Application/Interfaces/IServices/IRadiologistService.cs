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
    public interface IRadiologistService
    {
         Task<RegisterResult> Register(RegisterRadiologistDto radiologistDto);

        public Task<PagedResponse<RadiologistResponseDto>> GetAllAsync(int pageNumber, int pageSize);

         Task<PagedResponse<RadiologistResponseDto>> SearchAsync(RadiologistSearchDto searchDto);
    }
}
