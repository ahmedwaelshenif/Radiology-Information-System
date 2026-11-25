using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class CreateBookOrderDto
    {

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
