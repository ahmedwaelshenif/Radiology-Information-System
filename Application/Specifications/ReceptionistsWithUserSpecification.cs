using Domain.Entities;
using Domain.Specifications;

public class ReceptionistsWithUserSpecification : BaseSpecification<Receptionist>
{
    public ReceptionistsWithUserSpecification(int pageNumber, int pageSize)
        : base(r => true)
    {
        AddInclude(r => r.User);
        AddOrderBy(r => r.Id);
        ApplyPaging(pageNumber, pageSize);
    }
}