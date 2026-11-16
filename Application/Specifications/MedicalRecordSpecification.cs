
using Application.DTOs.RequestDto;
using Domain.Entities;
using Domain.Specifications;
using static System.Net.WebRequestMethods;

namespace Infrastructure.Specifications
{
    public class MedicalRecordSpecification : BaseSpecification<MedicalRecord>
    {
        public MedicalRecordSpecification(MedicalRecordSearchDto searchDto)
            : base(m =>
                (!searchDto.PatientId.HasValue ||
                 m.PatientId == searchDto.PatientId.Value)

                &&

                (string.IsNullOrWhiteSpace(searchDto.PatientName) ||
                 m.Patient.User.FullName.Contains(searchDto.PatientName))

                &&

                (string.IsNullOrWhiteSpace(searchDto.RadiologistName) ||
                 m.RecordedByUser.FullName.Contains(searchDto.RadiologistName))
            )
        {
            AddInclude(m => m.Patient);
            AddInclude(m => m.Patient.User);
            AddInclude(m => m.RecordedByUser);
            ApplyPaging(searchDto.PageNumber, searchDto.PageSize);
        }
    }
}
