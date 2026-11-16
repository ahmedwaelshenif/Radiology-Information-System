using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class TreatmentPlanSearchDto
    {
        public int? Id { get; set; }
        public int? PatientId { get; set; }
        public string? PlanTitle { get; set; }
        public DateOnly? Date { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
