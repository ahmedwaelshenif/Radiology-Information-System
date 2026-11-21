using Application.DTOs.ResponseDto;
using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.RequestDto;

namespace Application.Interfaces.IServices
{
    public interface IAuthService
    {
       Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
    }
}
