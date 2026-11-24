using Application.DTOs.RequestDto;
using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications
{
    public class RadiologyReportSearchSpecification : BaseSpecification<RadiologyReport>
    {
        public RadiologyReportSearchSpecification(RadiologyReportSearchDto filter)
            : base(r =>
                (!filter.ReportId.HasValue || r.Id == filter.ReportId.Value) &&

                (!filter.UserId.HasValue || r.Study.Order.Patient.UserId == filter.UserId.Value) &&

                (!filter.StudyId.HasValue || r.StudyId == filter.StudyId.Value) &&

                (!filter.CreatedByUserId.HasValue || r.CreatedByUserId == filter.CreatedByUserId.Value)
            )
        {
            AddInclude(r => r.Study);
            AddInclude(r => r.Study.Order);
            AddInclude(r => r.Study.Order.Patient);
            AddInclude(r => r.CreatedByUser);

            AddOrderBy(r => r.Id);

            ApplyPaging(filter.PageNumber, filter.PageSize);
        }
    }
}
