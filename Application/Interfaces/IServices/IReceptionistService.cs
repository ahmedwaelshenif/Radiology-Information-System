using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Enums;

namespace Application.Interfaces.IServices
{
    public interface IReceptionistService
    {
        public Task<RegisterResult> Register(RegisterReceptionistDto receptionistDto);
        public Task<PagedResponse<ReceptionistResponseDto>> GetAllAsync(int pageNumber, int pageSize);
        public Task<PagedResponse<ReceptionistResponseDto>> SearchAsync(ReceptionistSearchDto searchDto);
    }
}
