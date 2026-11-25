using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class BookOrderSearchDto: PagedRequestDto
    {
        public int?Id { get; set; }
        public int? PatientId { get; set; }
        public string? PatientName { get; set; }
        public Modality? Modality { get; set; }
        public BookOrderStatus? Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string? BodyPart { get; set; }

        public DateTime? OrderedAtFrom { get; set; }
        public DateTime? OrderedAtTo { get; set; }

        public DateTime? ScheduledAtFrom { get; set; }
        public DateTime? ScheduledAtTo { get; set; }

       
    }
}
