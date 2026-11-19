using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class RadiologyStudy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public RadiologyOrder Order { get; set; }

        [Required]
        public string StudyInstanceUID { get; set; } = string.Empty;

        public DateOnly? StudyDate { get; set; }

        [Required]
        public StudyStatus Status { get; set; } = StudyStatus.Pending;
        // Navigation properties
        public RadiologyReport? Report { get; set; }

        public ICollection<CDSSResult> CDSSResults { get; set; } = new List<CDSSResult>();
    }
}