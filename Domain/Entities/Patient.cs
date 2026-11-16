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

        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
        public ICollection<RadiologyOrder> RadiologyOrders { get; set; } = new List<RadiologyOrder>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
