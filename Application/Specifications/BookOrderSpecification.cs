using Application.DTOs.RequestDto;
using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications
{
    public class BookOrderSpecification : BaseSpecification<BookOrder>
    {
        public BookOrderSpecification(BookOrderSearchDto search)
            : base(x =>

                 (!search.Id.HasValue ||
                    x.Id == search.Id.Value) &&

                (!search.PatientId.HasValue ||
                    x.PatientId == search.PatientId.Value) &&

                (string.IsNullOrEmpty(search.PatientName) ||
                    x.Patient.User.FullName.Contains(search.PatientName)) &&

                (!search.PaymentMethod.HasValue ||x.PaymentMethod == search.PaymentMethod.Value) &&

                (!search.Modality.HasValue ||
                    x.Modality == search.Modality.Value) &&

                (!search.Status.HasValue ||
                    x.Status == search.Status.Value) &&

                (string.IsNullOrEmpty(search.BodyPart) ||
                    x.BodyPart.Contains(search.BodyPart)) &&

                (!search.OrderedAtFrom.HasValue ||
                    x.OrderedAt >= search.OrderedAtFrom.Value) &&

                (!search.OrderedAtTo.HasValue ||
                    x.OrderedAt <= search.OrderedAtTo.Value) &&

                (!search.ScheduledAtFrom.HasValue ||
                    x.ScheduledAt >= search.ScheduledAtFrom.Value) &&

                (!search.ScheduledAtTo.HasValue ||
                    x.ScheduledAt <= search.ScheduledAtTo.Value)
            )
        {
            AddInclude(x => x.Patient);
            AddInclude(x => x.Patient.User);

            AddOrderByDescending(x => x.OrderedAt);

            ApplyPaging(search.PageNumber,search.PageSize);
        }
    }
}