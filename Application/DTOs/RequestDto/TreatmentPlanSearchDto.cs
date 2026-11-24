using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class TreatmentPlanSearchDto
    {
        public int? Id { get; set; }
        public int? PatientId { get; set; }
        public int? UserId { get; set; }
        public string? PatientName { get; set; }
        public string? PlanTitle { get; set; }
        public DateOnly? Date { get; set; }
        public TreatmentPlanStatus? Status { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
