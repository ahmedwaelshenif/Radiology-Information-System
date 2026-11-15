using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDto
{
    public class RadiologistResponseDto
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? LicenseNumber { get; set; }

    }
}
