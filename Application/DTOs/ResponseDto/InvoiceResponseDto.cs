using System;
using Domain.Enums;

namespace Application.DTOs.ResponseDto
{
    public class InvoiceResponseDto
    {
        public int? InvoiceId { get; set; }

        public int? PatientId { get; set; }
        public string? PatientName { get; set; } = string.Empty;

        public int? OrderId { get; set; }
        public string? AccessionNumber { get; set; } = string.Empty;

        public string? ServiceName { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public InvoiceStatus? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
