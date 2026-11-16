using System;
using Domain.Enums;

namespace Application.DTOs.ResponseDto
{
    public class RadiologyOrderResponseDto
    {
        public int? OrderId { get; set; }

        public int ?PatientId { get; set; }
        public string? PatientName { get; set; } = string.Empty;

        public int? OrderedByUserId { get; set; }
        public string? OrderedByName { get; set; } = string.Empty;

        public string? Modality { get; set; } = string.Empty;
        public string? BodyPart { get; set; } = string.Empty;
        public OrderStatus? Status { get; set; }
        public string? AccessionNumber { get; set; } = string.Empty;

        public DateTime? OrderedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }
    }
}
