using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int RecordedByUserId { get; set; }
        public User RecordedByUser { get; set; }

        public string? History { get; set; }

        public string? ClinicalNotes { get; set; }
    }
}