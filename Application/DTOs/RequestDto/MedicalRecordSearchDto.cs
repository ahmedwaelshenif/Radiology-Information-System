using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class MedicalRecordSearchDto : PagedRequestDto
    {
        public int? UserId { get; set; }
        public int? PatientId { get; set; }

        public string? PatientName { get; set; }

        public string? RadiologistName { get; set; }

 
    }
}
