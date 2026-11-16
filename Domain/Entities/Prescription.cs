using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        [Required]
        public string Details { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}