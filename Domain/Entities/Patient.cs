using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Domain.Enums;

namespace Domain.Entities
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]

        public DateOnly DateOfBirth { get; set; }
        [Required]

        public Gender Gender { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
