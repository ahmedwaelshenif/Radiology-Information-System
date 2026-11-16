using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class HL7Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public RadiologyOrder Order { get; set; }

        [Required]
        public string Direction { get; set; } = string.Empty;

        [Required]
        public string MessageType { get; set; } = string.Empty;

        [Required]
        public HL7MessageStatus Status { get; set; } = HL7MessageStatus.Pending;

        [Required]
        public string RawMessage { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}