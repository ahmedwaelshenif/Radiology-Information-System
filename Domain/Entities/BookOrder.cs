using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class BookOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public Modality Modality { get; set; }

        [Required]
        public string BodyPart { get; set; } = string.Empty;

        [Required]
        public BookOrderStatus Status { get; set; } = BookOrderStatus.Pending;

        [Required]
        public DateTime OrderedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public DateTime ScheduledAt { get; set; }

    }
}
