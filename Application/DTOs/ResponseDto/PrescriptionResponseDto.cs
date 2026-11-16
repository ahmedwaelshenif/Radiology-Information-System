using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.ResponseDto
{
    public class PrescriptionResponseDto
    {
 
        public int? Id { get; set; }
        public string ? CreatedBy {  get; set; }
        public string? Details { get; set; } = string.Empty;
        public DateOnly? Date { get; set; }
        public int? TreatmentPlanId { get; set; }
        public PrescriptionStatus? Status { get; set; }
    }
}
