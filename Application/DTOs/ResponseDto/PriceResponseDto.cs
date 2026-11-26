using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDto
{
    public class PriceResponseDto
    {
        public Modality Modality { get; set; }
        public decimal Amount { get; set; }
    }
}
