using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.ResponseDto
{
    public class BookOrderResponseDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        public string PatientName { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public Modality Modality { get; set; }

        
        public string BodyPart { get; set; } = string.Empty;

        
        public BookOrderStatus Status { get; set; } = BookOrderStatus.Pending;

    
        public DateTime OrderedAt { get; set; } = DateTime.UtcNow;

      
        public DateTime ScheduledAt { get; set; }
    }
}
