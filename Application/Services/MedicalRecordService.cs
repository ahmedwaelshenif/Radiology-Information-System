using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Specifications.PatientSpecifications;
using Domain.Entities;
using Infrastructure.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        public MedicalRecordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(MedicalRecordRequestDto medicalRecordRequestDto,int recordedByUserId)
        {
            MedicalRecord MedicalRecord = new MedicalRecord { 
                ClinicalNotes=medicalRecordRequestDto.ClinicalNotes,
                Date= medicalRecordRequestDto.Date,
                PatientId=medicalRecordRequestDto.PatientId,
                RecordedByUserId = recordedByUserId
            };
            await _unitOfWork.Repository<MedicalRecord>().AddAsync(MedicalRecord);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResponse<MedicalRecordResponse>> SearchAsync(MedicalRecordSearchDto searchDto)
        {
            searchDto.PageNumber = Math.Max(1, searchDto.PageNumber);
            searchDto.PageSize = Math.Clamp(searchDto.PageSize, 1, 100);
            var specification = new MedicalRecordSpecification(searchDto);
            var medicalRecords = await _unitOfWork.Repository<MedicalRecord>().ListAsync(specification);
            int totalCount = await _unitOfWork.Repository<MedicalRecord>().CountAsync(specification);
            return new PagedResponse<MedicalRecordResponse>
            {
                Items = medicalRecords.Select(ToDto).ToList(),
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                TotalCount = totalCount
            };
        }




        //Helper Methods
        private MedicalRecordResponse ToDto(MedicalRecord medicalRecord) {
            return new MedicalRecordResponse
            {
                Id= medicalRecord.Id,
                PatientId = medicalRecord.PatientId,
                PatientName = medicalRecord.Patient.User.FullName,
                RecordedBy = medicalRecord.RecordedByUser.FullName,
                ClinicalNotes = medicalRecord.ClinicalNotes,
            };
        }
    }
}
