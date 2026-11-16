using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class MedicalRecordRequestDto
    {
        
        [Required]
        public int PatientId { get; set; }

        [Required]
        public string ClinicalNotes { get; set; }

        [Required]
        public DateOnly Date { get; set; }
    }
}
