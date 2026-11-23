using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int OrderId { get; set; }
        public RadiologyOrder Order { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}