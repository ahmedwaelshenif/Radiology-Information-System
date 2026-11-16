using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.ResponseDto
{
    public class MedicalRecordResponse
    {
        public int? Id { get; set; }
        public int? PatientId { get; set; }
        public string? PatientName { get; set; }
        public string? RecordedBy { get; set; }
        public string? ClinicalNotes { get; set; }
    }
}
