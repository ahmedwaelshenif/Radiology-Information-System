using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class Radiologist
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

    }
}
