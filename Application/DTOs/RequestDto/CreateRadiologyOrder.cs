using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.RequestDto
{
    public class CreateRadiologyOrderDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public Modality Modality { get; set; }    

        [Required]
        public string BodyPart { get; set; } = string.Empty;

        [Required]
        public DateTime ScheduledAt { get; set; }


        [Required]
        public PaymentMethod PaymentMethod { get; set; }

    }
}
