using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class PatientSearchDto : PagedRequestDto
    {
        public int? Id { get; set; }

        public int? UserId { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public bool? IsActive { get; set; }

        public string? PhoneNumber { get; set; }

    


    }
}
