using Domain.Entities;
using Domain.Specifications;
using static System.Net.WebRequestMethods;

public class RadiologyOrderWithIncludeSpecification : BaseSpecification<RadiologyOrder>
{
    public RadiologyOrderWithIncludeSpecification(int pageNumber, int pageSize)
        : base(r => true)
    {
        AddInclude(o => o.Patient.User);
        AddInclude(o => o.OrderedByUser);
        AddInclude(o => o.Study);
        AddOrderBy(o => o.Id);

        ApplyPaging(pageNumber, pageSize);
    }
}