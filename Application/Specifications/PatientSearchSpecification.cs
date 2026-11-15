using Application.DTOs.RequestDto;
using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications.PatientSpecifications
{
    public class PatientSearchSpecification : BaseSpecification<Patient>
    {
        public PatientSearchSpecification(PatientSearchDto filter)
        {
            AddCriteria(p =>
                (!filter.Id.HasValue || p.Id == filter.Id.Value) &&

                (!filter.UserId.HasValue || p.UserId == filter.UserId.Value) &&

                (string.IsNullOrEmpty(filter.FullName) ||
                    p.User.FullName.Contains(filter.FullName)) &&

                (string.IsNullOrEmpty(filter.Email) ||
                    p.User.Email.Contains(filter.Email)) &&

                (!filter.DateOfBirth.HasValue ||
                    p.DateOfBirth == filter.DateOfBirth.Value) &&

                (!filter.Gender.HasValue ||
                    p.Gender == filter.Gender.Value) &&

                (string.IsNullOrEmpty(filter.PhoneNumber) ||
                    (p.PhoneNumber != null &&
                     p.PhoneNumber.Contains(filter.PhoneNumber)))
            );

            AddInclude(p => p.User);

            AddOrderBy(p => p.Id);

            ApplyPaging(filter.PageNumber, filter.PageSize);
        }
    }
}