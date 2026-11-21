using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class MedicalRecordSearchDto
    {
        public int? UserId { get; set; }
        public int? PatientId { get; set; }

        public string? PatientName { get; set; }

        public string? RadiologistName { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
