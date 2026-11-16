using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class RadiologyOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int OrderedByUserId { get; set; }
        public User OrderedByUser { get; set; }

        [Required]
        public string Modality { get; set; } = string.Empty;

        [Required]
        public string BodyPart { get; set; } = string.Empty;

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Ordered;

        [Required]
        public string AccessionNumber { get; set; } = string.Empty;

        public DateTime OrderedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ScheduledAt { get; set; }

        // Navigation properties
        public ICollection<RadiologyStudy> Studies { get; set; } = new List<RadiologyStudy>();

        public Invoice? Invoice { get; set; }

        public ICollection<HL7Message> HL7Messages { get; set; } = new List<HL7Message>();
    }
}