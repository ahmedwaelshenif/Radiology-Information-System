
using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications
{
    public class RadiologyStudyWithDetailsSpecification
        : BaseSpecification<RadiologyStudy>
    {
        public RadiologyStudyWithDetailsSpecification(int studyId,int? UserID=null)
            : base(s => s.Id == studyId && (!UserID.HasValue || s.Order.Patient.UserId == UserID.Value))
        {
            AddInclude(s => s.Order);

            AddInclude(s => s.Order.Patient);
            AddInclude(s => s.Order.Patient.User);

            AddInclude(s => s.Order.OrderedByUser);

            AddInclude(s => s.Report);

            AddInclude(s => s.CDSSResults);
        }
    }
}
