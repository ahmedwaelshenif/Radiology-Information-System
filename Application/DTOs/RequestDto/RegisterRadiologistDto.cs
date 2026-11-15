using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class RegisterRadiologistDto
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
        [MaxLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;
    }
}
