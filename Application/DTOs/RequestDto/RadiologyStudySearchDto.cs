using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public  class RadiologyStudySearchDto
    {
        public int? StudyId { get; set; }
        public int? OrderId { get; set; }
        public int? PatientId { get; set; }
        public string? PatientName { get; set; }
        public string? StudyInstanceUID { get; set; }
        public StudyStatus? Status { get; set; }
        public DateOnly? StudyDateFrom { get; set; }
        public DateOnly? StudyDateTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
