using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDto
{
    public class TreatmentPlanResponseDto
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string PlanTitle { get; set; }

        public string PlanDetails { get; set; }
    }
}
