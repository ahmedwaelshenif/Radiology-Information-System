
using Application.DTOs.RequestDto;
using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications
{
    public class RadiologyStudySearchSpecification
        : BaseSpecification<RadiologyStudy>
    {
        public RadiologyStudySearchSpecification(
            RadiologyStudySearchDto filter)
            : base(s =>
                (!filter.StudyId.HasValue ||
                    s.Id == filter.StudyId.Value) &&

                (!filter.OrderId.HasValue ||
                    s.OrderId == filter.OrderId.Value) &&

                (!filter.PatientId.HasValue ||
                    s.Order.PatientId == filter.PatientId.Value) &&

                (string.IsNullOrEmpty(filter.PatientName) ||
                    s.Order.Patient.User.FullName.Contains(filter.PatientName))&&

                (string.IsNullOrEmpty(filter.StudyInstanceUID) ||
                    s.StudyInstanceUID.Contains(filter.StudyInstanceUID)) &&

                (!filter.Status.HasValue ||
                    s.Status == filter.Status.Value) &&

                (!filter.StudyDateFrom.HasValue ||
                    (s.StudyDate.HasValue &&
                     s.StudyDate.Value >= filter.StudyDateFrom.Value)) &&

                (!filter.StudyDateTo.HasValue ||
                    (s.StudyDate.HasValue &&
                     s.StudyDate.Value <= filter.StudyDateTo.Value))
            )
        {
            AddInclude(s => s.Order);
            AddInclude(s => s.Order.Patient);
            AddInclude(s => s.Order.Patient.User);
            
            AddOrderByDescending(s => s.Id);

            ApplyPaging(filter.PageNumber, filter.PageSize);
        }
    }
}
