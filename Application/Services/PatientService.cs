using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Specifications.PatientSpecifications;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Identity.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Enums;


namespace Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        public PatientService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterResult> Register(RegisterPatientDto patientDto)
        {
            bool exist= await _unitOfWork.Repository<User>().AnyAsync(u=>u.Email==patientDto.Email);
            if (exist) return RegisterResult.EmailAlreadyExists;
            var user = new User
            {
                FullName = patientDto.FullName,
                Email = patientDto.Email,
                Role = Role.Patient
            };
            user.PasswordHash = _passwordHasher.HashPassword(user,patientDto.Password);

            var patient = new Patient
            {
                User=user,
                PhoneNumber = patientDto.PhoneNumber,
                Gender = patientDto.Gender,
                DateOfBirth = patientDto.DateOfBirth,
            };

            await _unitOfWork.Repository<Patient>().AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();
            return RegisterResult.Success;

        }

        public async Task<PagedResponse<PatientResponseDto>> GetAllAsync(int pageNumber,int pageSize)
        {
            var specification = new PatientsWithUserSpecification(pageNumber,pageSize);
            var patients = await _unitOfWork.Repository<Patient>().ListAsync(specification);
            int totalCount = await _unitOfWork.Repository<Patient>().CountAsync(specification);
            return new PagedResponse<PatientResponseDto> {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = patients.Select(ToDto).ToList()
            };
        }

        public async Task<PagedResponse<PatientResponseDto>> SearchAsync(PatientSearchDto searchDto)
        {
            searchDto.PageNumber = Math.Max(1, searchDto.PageNumber);
            searchDto.PageSize = Math.Clamp(searchDto.PageSize, 1, 100);
            var specification=new PatientSearchSpecification(searchDto);
            var patients = await _unitOfWork.Repository<Patient>().ListAsync(specification);
            int totalCount = await _unitOfWork.Repository<Patient>().CountAsync(specification);
            return new PagedResponse<PatientResponseDto>
            {
                Items = patients.Select(ToDto).ToList(),
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                TotalCount = totalCount
            };
        }

        //Helper Methods

        private PatientResponseDto ToDto(Patient patient)
        {
            return  new PatientResponseDto { 
                Email=patient.User.Email,
                FullName=patient.User.FullName,
                DateOfBirth=patient.DateOfBirth,
                Gender=patient.Gender,
                PhoneNumber=patient.PhoneNumber,
                Id=patient.Id,
                UserId=patient.UserId, 
            };
        }

       
    }
}
