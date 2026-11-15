using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class Receptionist
    {
        [Key]
        public int Id { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
