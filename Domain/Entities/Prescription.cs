using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public string Details { get; set; } = string.Empty;

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public int TreatmentPlanId { get; set; }
        public TreatmentPlan TreatmentPlan { get; set; }
        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Active;

    }
}