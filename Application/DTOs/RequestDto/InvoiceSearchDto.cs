using System;
using Domain.Enums;

namespace Application.DTOs.RequestDto
{
    public class InvoiceSearchDto: PagedRequestDto
    {
        public int? InvoiceId { get; set; }
        public int? PatientId { get; set; }
        public int? UserId { get; set; }
       
        public int? OrderId { get; set; }

        public string? PatientName { get; set; }
        public string? AccessionNumber { get; set; }
        public string? ServiceName { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }

      
    }
}
