using Domain.Entities;
using Domain.Specifications;

public class PatientsWithUserSpecification : BaseSpecification<Patient>
{
    public PatientsWithUserSpecification(int pageNumber, int pageSize)
        : base(p => true)
    {
        AddInclude(p => p.User);
        AddOrderBy(p => p.Id);
        ApplyPaging(pageNumber, pageSize);
    }
}