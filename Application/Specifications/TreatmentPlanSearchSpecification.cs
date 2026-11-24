using Domain.Entities;
using Domain.Specifications;
using Application.DTOs.RequestDto;

namespace Application.Specifications
{
    public class TreatmentPlanSearchSpecification : BaseSpecification<TreatmentPlan>
    {
        public TreatmentPlanSearchSpecification(TreatmentPlanSearchDto searchDto)
            : base(tp =>
                (!searchDto.PatientId.HasValue ||
                    tp.PatientId == searchDto.PatientId.Value)
                &&

                  (!searchDto.UserId.HasValue ||
                    tp.Patient.UserId == searchDto.UserId.Value)

                && (!searchDto.Id.HasValue ||
                    tp.Id == searchDto.Id.Value)

                 && (!searchDto.Status.HasValue ||
                    tp.Status == searchDto.Status.Value)

                && (string.IsNullOrEmpty(searchDto.PlanTitle) ||
                    tp.PlanTitle.Contains(searchDto.PlanTitle))

                && (string.IsNullOrEmpty(searchDto.PatientName) ||
                    tp.Patient.User.FullName.Contains(searchDto.PatientName))

                && (!searchDto.Date.HasValue ||
                    tp.Date == searchDto.Date.Value)
             
            )
        {
            AddOrderByDescending(tp => tp.Date);
            ApplyPaging(searchDto.PageNumber, searchDto.PageSize);
            AddInclude(tp => tp.Patient.User);
        }
    }
}