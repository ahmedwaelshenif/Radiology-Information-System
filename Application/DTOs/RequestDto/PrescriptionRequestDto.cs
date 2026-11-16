using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class PrescriptionRequestDto
    {

        [Required]
        public string Details { get; set; } = string.Empty;

        [Required]
        public int TreatmentPlanId { get; set; }
        
    }
}
