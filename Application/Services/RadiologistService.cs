using Application.DTOs;
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Specifications;
using Application.Specifications.PatientSpecifications;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class RadiologistService : IRadiologistService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        public RadiologistService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }
        public async Task<RegisterResult> Register(RegisterRadiologistDto radiologistDto)
        {
            bool exist = await _unitOfWork.Repository<User>().AnyAsync(u => u.Email == radiologistDto.Email);
            if (exist) return RegisterResult.EmailAlreadyExists;

           exist = await _unitOfWork.Repository<Radiologist>().AnyAsync(r => r.LicenseNumber == radiologistDto.LicenseNumber);
           if (exist) return RegisterResult.LicenseAlreadyExists; 

            var user = new User
            {
                FullName = radiologistDto.FullName,
                Email = radiologistDto.Email,
                Role = Role.Radiologist
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, radiologistDto.Password);

            var radiologist = new Radiologist
            {
                User = user,
                LicenseNumber = radiologistDto.LicenseNumber,
            };

            await _unitOfWork.Repository<Radiologist>().AddAsync(radiologist);
            await _unitOfWork.SaveChangesAsync();
            return RegisterResult.Success; 

        }



        public async Task<PagedResponse<RadiologistResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {

            var specification = new RadiologistsWithUserSpecification(pageNumber, pageSize);
            var radiologists = await _unitOfWork.Repository<Radiologist>().ListAsync(specification);
            int totalCount = await _unitOfWork.Repository<Radiologist>().CountAsync(specification);
            return new PagedResponse<RadiologistResponseDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = radiologists.Select(ToDto).ToList()
            };
        }

        public async Task<PagedResponse<RadiologistResponseDto>> SearchAsync(RadiologistSearchDto searchDto)
        {
            var specification = new RadiologistSearchSpecification(searchDto);
            var radiologists = await _unitOfWork.Repository<Radiologist>().ListAsync(specification);
            int totalCount = await _unitOfWork.Repository<Radiologist>().CountAsync(specification);
            return new PagedResponse<RadiologistResponseDto>
            {
                Items = radiologists.Select(ToDto).ToList(),
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                TotalCount = totalCount
            };
        }


        //Helper Methods

        private RadiologistResponseDto ToDto(Radiologist radiologist)
        {
            return new RadiologistResponseDto
            {
                Email = radiologist.User.Email,
                FullName = radiologist.User.FullName,
                Id = radiologist.Id,
                UserId = radiologist.User.Id,
                LicenseNumber = radiologist.LicenseNumber
            };
        }
    }
}
