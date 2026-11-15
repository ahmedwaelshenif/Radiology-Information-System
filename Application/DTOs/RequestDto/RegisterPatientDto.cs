using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class RegisterPatientDto
    {

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
