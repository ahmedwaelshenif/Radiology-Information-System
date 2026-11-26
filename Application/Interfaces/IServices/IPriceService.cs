using Application.DTOs.ResponseDto;
using Domain.Enums;

using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IPriceService
    {
        Task UpdateModalityPrice(Modality mod, decimal amount);
        Task<List<PriceResponseDto>> GetAllAsync();
    }
}
