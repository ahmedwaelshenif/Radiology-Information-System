namespace Application.DTOs.ResponseDto
{
    // The PDF bytes are intentionally not part of this DTO.
    // Use IRadiologyReportService.GetPdfAsync to download the file.
    public class RadiologyReportResponseDto
    {
        public int? ReportId { get; set; }

        public int? StudyId { get; set; }
        public string? StudyInstanceUID { get; set; } = string.Empty;

        public int? CreatedByUserId { get; set; }
        public string? CreatedByName { get; set; } = string.Empty;
    }
}
