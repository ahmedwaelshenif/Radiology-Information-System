using Application.DTOs.RequestDto;
using Domain.Entities;
using Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Specifications
{
    internal class RadiologistSearchSpecification: BaseSpecification<Radiologist>
    {
        public RadiologistSearchSpecification(RadiologistSearchDto filter)
        {
            AddCriteria(r =>
                (!filter.Id.HasValue || r.Id == filter.Id.Value) &&

                (!filter.UserId.HasValue || r.UserId == filter.UserId.Value) &&
                (!filter.IsActive.HasValue || r.User.IsActive == filter.IsActive.Value) &&

                (string.IsNullOrWhiteSpace(filter.FullName) ||
                    r.User.FullName.Contains(filter.FullName)) &&

                (string.IsNullOrWhiteSpace(filter.Email) ||
                    r.User.Email.Contains(filter.Email)) &&

                (string.IsNullOrWhiteSpace(filter.LicenseNumber) ||
                    (r.LicenseNumber != null &&
                     r.LicenseNumber.Contains(filter.LicenseNumber)))
            );

            AddInclude(r => r.User);
            AddOrderBy(r => r.Id);

            ApplyPaging(filter.PageNumber, filter.PageSize);
        }
    }
}
