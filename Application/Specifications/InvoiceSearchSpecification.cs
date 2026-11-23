using Application.DTOs.RequestDto;
using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications
{
    public class InvoiceSearchSpecification : BaseSpecification<Invoice>
    {
        public InvoiceSearchSpecification(InvoiceSearchDto filter)
            : base(i =>
                (!filter.InvoiceId.HasValue || i.Id == filter.InvoiceId.Value) &&

                (!filter.UserId.HasValue || i.Patient.UserId == filter.UserId.Value) &&


                (!filter.PatientId.HasValue || i.PatientId == filter.PatientId.Value) &&

                (!filter.OrderId.HasValue || i.OrderId == filter.OrderId.Value) &&

                (string.IsNullOrWhiteSpace(filter.PatientName) ||
                    i.Patient.User.FullName.Contains(filter.PatientName)) &&

                (string.IsNullOrWhiteSpace(filter.AccessionNumber) ||
                    i.Order.AccessionNumber.Contains(filter.AccessionNumber)) &&

                (string.IsNullOrWhiteSpace(filter.ServiceName) ||
                    i.ServiceName.Contains(filter.ServiceName)) &&

                (!filter.PaymentMethod.HasValue || i.PaymentMethod == filter.PaymentMethod.Value) &&

                (!filter.Status.HasValue || i.Status == filter.Status.Value) &&

                (!filter.MinAmount.HasValue || i.Amount >= filter.MinAmount.Value) &&
                (!filter.MaxAmount.HasValue || i.Amount <= filter.MaxAmount.Value) &&

                (!filter.CreatedFrom.HasValue || i.CreatedAt >= filter.CreatedFrom.Value) &&
                (!filter.CreatedTo.HasValue || i.CreatedAt <= filter.CreatedTo.Value) &&

                (!filter.PaidFrom.HasValue ||
                    (i.PaidAt != null && i.PaidAt >= filter.PaidFrom.Value)) &&
                (!filter.PaidTo.HasValue ||
                    (i.PaidAt != null && i.PaidAt <= filter.PaidTo.Value))
            )
        {
            AddInclude(i => i.Patient.User);
            AddInclude(i => i.Order);
            AddOrderBy(i => i.Id);

            ApplyPaging(filter.PageNumber, filter.PageSize);
        }
    }
}
