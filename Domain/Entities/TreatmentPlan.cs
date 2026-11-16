using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class TreatmentPlan
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
        public string PlanTitle { get; set;  }

        [Required]
        public string PlanDetails { get; set; } = string.Empty;

        public DateOnly Date { get; set; } 

        [Required]
        public TreatmentPlanStatus Status { get; set; } = TreatmentPlanStatus.Draft;

       
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();


    }
}