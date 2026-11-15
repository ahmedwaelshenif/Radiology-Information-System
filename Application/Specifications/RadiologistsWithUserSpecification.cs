using Domain.Entities;
using Domain.Specifications;

public class RadiologistsWithUserSpecification : BaseSpecification<Radiologist>
{
    public RadiologistsWithUserSpecification(int pageNumber, int pageSize)
        : base(r => true)
    {
        AddInclude(r => r.User);
        AddOrderBy(r => r.Id);
        ApplyPaging(pageNumber, pageSize);
    }
}