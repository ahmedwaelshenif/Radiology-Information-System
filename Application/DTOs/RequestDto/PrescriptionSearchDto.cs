using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class PrescriptionSearchDto
    {
        public int? Id { get; set; }

        public int? TreatmentPlanId { get; set; }
        public int? PatientId { get; set; }

        public string? PlanTitle { get; set; }

        public int? CreatedByUserId { get; set; }

        public DateOnly? FromDate { get; set; }

        public DateOnly? ToDate { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}
