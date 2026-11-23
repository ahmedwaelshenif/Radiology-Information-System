using Application.DTOs.RequestDto;
using Application.Specifications;
using Domain.Entities;
using Domain.Specifications;

public class PrescriptionSearchSpecification
    : BaseSpecification<Prescription>
{
    public PrescriptionSearchSpecification(PrescriptionSearchDto searchDto)
        : base(p =>
            (!searchDto.Id.HasValue || p.Id == searchDto.Id.Value) &&

            (!searchDto.PatientId.HasValue || p.TreatmentPlan.PatientId == searchDto.PatientId.Value) &&

            (!searchDto.TreatmentPlanId.HasValue ||
                p.TreatmentPlanId == searchDto.TreatmentPlanId.Value) &&

             (!searchDto.UserId.HasValue ||
                p.TreatmentPlan.Patient.UserId == searchDto.UserId.Value) &&

             (string.IsNullOrWhiteSpace(searchDto.PlanTitle) ||
                    p.TreatmentPlan.PlanTitle.Contains(searchDto.PlanTitle)) &&

            (!searchDto.CreatedByUserId.HasValue ||
                p.CreatedByUserId == searchDto.CreatedByUserId.Value) &&

            (!searchDto.FromDate.HasValue ||
                p.Date >= searchDto.FromDate.Value) &&

            (!searchDto.ToDate.HasValue ||
                p.Date <= searchDto.ToDate.Value)
        )
    {
        AddInclude(p => p.CreatedByUser);
        AddInclude(p => p.TreatmentPlan);
        AddInclude(p => p.TreatmentPlan.Patient);
        AddOrderByDescending(p => p.Date);
        ApplyPaging(searchDto.PageNumber, searchDto.PageSize);
    }
}