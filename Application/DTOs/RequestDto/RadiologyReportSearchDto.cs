namespace Application.DTOs.RequestDto
{
    public class RadiologyReportSearchDto : PagedRequestDto
    {
        public int? ReportId { get; set; }

        public int? UserId { get; set; }
        public int? StudyId { get; set; }
        public int? CreatedByUserId { get; set; }

       
    }
}
