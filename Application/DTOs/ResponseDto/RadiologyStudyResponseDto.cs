using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.ResponseDto
{
    public class RadiologyStudyResponseDto
    {
        
        public int? Id { get; set; }

        public int? PatientId { get; set; }
        public string? PatientName { get; set; }
        public int? OrderId { get; set; }

        public string? StudyInstanceUID { get; set; } = string.Empty;

        public DateOnly? StudyDate { get; set; }

        public StudyStatus? Status { get; set; } = StudyStatus.Pending;
 
        public RadiologyReport? Report { get; set; }

        public ICollection<CDSSResult>? CDSSResults { get; set; } = new List<CDSSResult>();
    }
}
