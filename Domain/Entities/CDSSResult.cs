using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class CDSSResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudyId { get; set; }
        public RadiologyStudy Study { get; set; }

        [Required]
        public string RuleOrModel { get; set; } = string.Empty;

        // Stored as a JSON string
        [Required]
        public string Output { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}