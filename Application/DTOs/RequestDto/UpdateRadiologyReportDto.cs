using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.RequestDto
{
    public class UpdateRadiologyReportDto
    {
        [Required]
        public List<string> Fields { get; set; } = new();
    }
}
