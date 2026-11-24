using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDto
{
    public class PatientResponseDto
    {
        public string? Email { get;set;  }
        public string? FullName { get; set; }
        public int? Id { get; set; }

        public bool ? IsActive { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    
        public Gender? Gender { get; set; }

        public string? PhoneNumber { get; set; }

        public int? UserId { get; set; }

    }
}
