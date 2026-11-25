using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class PrescriptionSearchDto : PagedRequestDto
    {
        public int? Id { get; set; }

        public int? TreatmentPlanId { get; set; }
        public int? PatientId { get; set; }
        public int? UserId { get; set; }

        public string? PlanTitle { get; set; }

        public int? CreatedByUserId { get; set; }

        public DateOnly? FromDate { get; set; }

        public DateOnly? ToDate { get; set; }

  
    }
}
