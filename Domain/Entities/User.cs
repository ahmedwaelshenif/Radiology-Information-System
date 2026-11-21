using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Identity;


namespace Domain.Entities
{
    public class User : IdentityUser<int>    {

        [Required]
        public string FullName { get; set; }

        [Required]
        public Role Role { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Patient? Patient { get; set; }

        public Radiologist? Radiologist { get; set; }

        public Receptionist? Receptionist { get; set; }

    }
}
