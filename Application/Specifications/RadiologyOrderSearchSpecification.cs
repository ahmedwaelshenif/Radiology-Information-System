using Application.DTOs.RequestDto;
using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications
{
    public class RadiologyOrderSearchSpecification : BaseSpecification<RadiologyOrder>
    {
        public RadiologyOrderSearchSpecification(RadiologyOrderSearchDto filter)
            : base(o =>
                (!filter.OrderId.HasValue || o.Id == filter.OrderId.Value) &&

                (!filter.PatientId.HasValue || o.PatientId == filter.PatientId.Value) &&

                (!filter.OrderedByUserId.HasValue || o.OrderedByUserId == filter.OrderedByUserId.Value) &&

                (string.IsNullOrWhiteSpace(filter.PatientName) ||
                    o.Patient.User.FullName.Contains(filter.PatientName)) &&

                (string.IsNullOrWhiteSpace(filter.OrderdByName) ||
                    o.OrderedByUser.FullName.Contains(filter.OrderdByName)) &&

                (!filter.Modality.HasValue ||
                    o.Modality == filter.Modality.Value)&&

                (string.IsNullOrWhiteSpace(filter.BodyPart) ||
                    o.BodyPart.Contains(filter.BodyPart)) &&

                (string.IsNullOrWhiteSpace(filter.AccessionNumber) ||
                    o.AccessionNumber.Contains(filter.AccessionNumber)) &&

                (!filter.Status.HasValue || o.Status == filter.Status.Value) &&

                (!filter.OrderedFrom.HasValue || o.OrderedAt >= filter.OrderedFrom.Value) &&
                (!filter.OrderedTo.HasValue || o.OrderedAt <= filter.OrderedTo.Value) &&

                (!filter.ScheduledFrom.HasValue ||
                    (o.ScheduledAt != null && o.ScheduledAt >= filter.ScheduledFrom.Value)) &&
                (!filter.ScheduledTo.HasValue ||
                    (o.ScheduledAt != null && o.ScheduledAt <= filter.ScheduledTo.Value))
            )
        {
            AddInclude(o => o.Patient.User);
            AddInclude(o => o.OrderedByUser);
            AddOrderBy(o => o.Id);

            ApplyPaging(filter.PageNumber, filter.PageSize);
        }
    }
}