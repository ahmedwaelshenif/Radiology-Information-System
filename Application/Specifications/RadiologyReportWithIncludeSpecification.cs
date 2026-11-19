using Application.DTOs.RequestDto;
using Domain.Entities;
using Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Specifications
{
    public class RadiologyReportWithIncludeSpecification: BaseSpecification<RadiologyReport>
    {
        public RadiologyReportWithIncludeSpecification(int PageNumber, int PageSize) :base(r => true)
        {
            AddInclude(r => r.Study);
            AddInclude(r => r.CreatedByUser);
            AddOrderBy(r => r.Id);
            ApplyPaging(PageNumber, PageSize);
        }
    }
}
