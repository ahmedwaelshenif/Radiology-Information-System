using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Specifications;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class RadiologyStudyService : IRadiologyStudyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RadiologyStudyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<RadiologyStudyResponseDto> GetByIdAsync(int id)
        {
            var specification = new RadiologyStudyWithDetailsSpecification(id);
            var study= await _unitOfWork.Repository<RadiologyStudy>().GetEntityWithSpecAsync(specification);
            if (study == null)
                throw new KeyNotFoundException($"Radiology study {id} was not found.");
            return ToDto(study);
        }

        public async Task<RadiologyStudyResponseDto> GetMyStudyByIdAsync( int id, int userId)
        {
            var specification = new RadiologyStudyWithDetailsSpecification(id,userId);
            var study = await _unitOfWork.Repository<RadiologyStudy>().GetEntityWithSpecAsync(specification);

            if (study == null)
                throw new KeyNotFoundException("Radiology study not found.");

            return ToDto(study);
        }

        public async Task<PagedResponse<RadiologyStudyResponseDto>> GetMyStudiesAsync(int pageSize, int pageNumber, int userId)
        {

            return await SearchAsync(new RadiologyStudySearchDto { UserId = userId,PageSize=pageSize,PageNumber=pageNumber });
        }

        public async Task<PagedResponse<RadiologyStudyResponseDto>> SearchAsync(RadiologyStudySearchDto searchDto)
        {
            var specification = new RadiologyStudySearchSpecification(searchDto);
            var studies = await _unitOfWork.Repository<RadiologyStudy>().ListAsync(specification);
            int totalCount= await _unitOfWork.Repository<RadiologyStudy>().CountAsync(specification);

            return new PagedResponse<RadiologyStudyResponseDto> { 
                Items=studies.Select(ToDto).ToList(),
                TotalCount=totalCount,
                PageNumber=searchDto.PageNumber,
                PageSize=searchDto.PageSize,
            };
        }

        public async Task<PagedResponse<RadiologyStudyResponseDto>> SearchMyStudiesAsync(int userId,RadiologyStudySearchDto searchDto)
        {
            searchDto.PatientId = null;
            searchDto.UserId = userId;
            return await SearchAsync(searchDto);
        }

        public async Task UpdateStatusAsync(StudyStatus status, int id)
        {
            var study=await _unitOfWork.Repository<RadiologyStudy>().GetByIdAsync(id);
            if (study == null)
                throw new KeyNotFoundException($"Radiology study {id} was not found.");


            if(study.Status== StudyStatus.Completed || study.Status == StudyStatus.Cancelled || status== StudyStatus.Pending)
                throw new InvalidOperationException($"Cannot change radiology study status from {study.Status} to {status}.");

            if (study.Status == StudyStatus.Pending && status == StudyStatus.Completed)
                throw new InvalidOperationException($"Cannot change radiology study status from {study.Status} to {status}.");

            study.Status = status;
            if(study.Status== StudyStatus.Pending && status== StudyStatus.AwaitingReport)
            {
                var order = await _unitOfWork.Repository<RadiologyOrder>().GetByIdAsync(study.OrderId);
                order.Status = OrderStatus.Completed;
            }
            await _unitOfWork.SaveChangesAsync();
        
        }
        public async Task<int> CountByStatus(StudyStatus ?status=null)
        { 
            if(status==null)
                return await _unitOfWork.Repository<RadiologyStudy>().CountAsync();
            else
            {
                return await _unitOfWork.Repository<RadiologyStudy>().CountAsync(s=>s.Status==status);
            }
        }
     




        //Helper Methods
        private RadiologyStudyResponseDto ToDto(RadiologyStudy study) {
            return new RadiologyStudyResponseDto { 
                Id= study.Id,
                Status= study.Status,
                StudyDate= study.StudyDate,
                StudyInstanceUID= study.StudyInstanceUID,
                CDSSResults= study.CDSSResults,
                PatientId=study.Order.PatientId,
                PatientName=study.Order.Patient.User.FullName,
                OrderId=study.OrderId,
                Report=study.Report
            };
        }
    }
}
