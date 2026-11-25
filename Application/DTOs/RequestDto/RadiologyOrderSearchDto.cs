using System;
using Domain.Enums;

namespace Application.DTOs.RequestDto
{
    public class RadiologyOrderSearchDto : PagedRequestDto
    {
        public int? OrderId { get; set; }
        public int? PatientId { get; set; }
        public int? UserId { get; set; }
        public int? OrderedByUserId { get; set; }

        public string? PatientName { get; set; }

        public string? OrderdByName { get; set; }
        public Modality? Modality { get; set; }
        public string? BodyPart { get; set; }
        public string? AccessionNumber { get; set; }
        public OrderStatus? Status { get; set; }

        public DateTime? OrderedFrom { get; set; }
        public DateTime? OrderedTo { get; set; }

        public DateTime? ScheduledFrom { get; set; }
        public DateTime? ScheduledTo { get; set; }


    }
}
