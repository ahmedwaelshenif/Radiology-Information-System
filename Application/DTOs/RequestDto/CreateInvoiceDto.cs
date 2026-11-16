using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.RequestDto
{
    public class CreateInvoiceDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
