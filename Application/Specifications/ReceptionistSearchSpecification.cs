using Application.DTOs.RequestDto;
using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications.ReceptionistSpecifications
{
    public class ReceptionistSearchSpecification
        : BaseSpecification<Receptionist>
    {
        public ReceptionistSearchSpecification(ReceptionistSearchDto filter)
        {
            AddCriteria(r =>
                (!filter.Id.HasValue || r.Id == filter.Id.Value) &&
                (!filter.IsActive.HasValue || r.User.IsActive == filter.IsActive.Value) &&

                (!filter.UserId.HasValue || r.UserId == filter.UserId.Value) &&

                (string.IsNullOrWhiteSpace(filter.FullName) ||
                    r.User.FullName.Contains(filter.FullName)) &&

                (string.IsNullOrWhiteSpace(filter.Email) ||
                    r.User.Email.Contains(filter.Email)) &&

                (string.IsNullOrWhiteSpace(filter.PhoneNumber) ||
                    (r.PhoneNumber != null &&
                     r.PhoneNumber.Contains(filter.PhoneNumber)))
            );

            AddInclude(r => r.User);
            AddOrderBy(r => r.Id);

            ApplyPaging(filter.PageNumber, filter.PageSize);
        }
    }
}