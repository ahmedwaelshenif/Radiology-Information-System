using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.RequestDto
{
    public class CreateRadiologyReportDto
    {
        [Required]
        public int StudyId { get; set; }

        [Required]
        public List<string> Fields { get; set; } = new();

    }
}
