using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications
{
    public class InvoiceWithIncludeSpecification : BaseSpecification<Invoice>
    {
        public InvoiceWithIncludeSpecification(int pageNumber, int pageSize)
            : base(i => true)
        {
            AddInclude(i => i.Patient.User);
            AddInclude(i => i.Order);
            AddOrderBy(i => i.Id);

            ApplyPaging(pageNumber, pageSize);
        }
    }
}
