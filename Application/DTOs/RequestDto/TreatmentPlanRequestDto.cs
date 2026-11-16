using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class TreatmentPlanRequestDto
    {
        public int PatientId { get; set; }

        public string PlanTitle { get; set; }

        public string PlanDetails { get; set; } = string.Empty;

    }
}
