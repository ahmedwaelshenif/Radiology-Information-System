namespace Application.DTOs.RequestDto
{
    public class RadiologyReportSearchDto
    {
        public int? ReportId { get; set; }

        public int? UserId { get; set; }
        public int? StudyId { get; set; }
        public int? CreatedByUserId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
