using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Specifications;
using Application.Specifications.ReceptionistSpecifications;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class ReceptionistService:IReceptionistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        public ReceptionistService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterResult> Register(RegisterReceptionistDto receptionistDto)
        {
            bool exist = await _unitOfWork.Repository<User>().AnyAsync(u => u.Email == receptionistDto.Email);
            if (exist) return RegisterResult.EmailAlreadyExists;
            var user = new User
            {
                FullName = receptionistDto.FullName,
                Email = receptionistDto.Email,
                Role = Role.Receptionist
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, receptionistDto.Password);

            var receptionist = new Receptionist
            {
                User = user,
                PhoneNumber = receptionistDto.PhoneNumber,
            };

            await _unitOfWork.Repository<Receptionist>().AddAsync(receptionist);
            await _unitOfWork.SaveChangesAsync();
            return RegisterResult.Success;

        }


        public async Task<PagedResponse<ReceptionistResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var specification = new ReceptionistsWithUserSpecification(pageNumber, pageSize);
            var receptionists = await _unitOfWork.Repository<Receptionist>().ListAsync(specification);
            int totalCount = await _unitOfWork.Repository<Receptionist>().CountAsync(specification);
            return new PagedResponse<ReceptionistResponseDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = receptionists.Select(ToDto).ToList()
            };
        }


        public async Task<PagedResponse<ReceptionistResponseDto>> SearchAsync(ReceptionistSearchDto searchDto)
        {
            var specification = new ReceptionistSearchSpecification(searchDto);
            var receptionists = await _unitOfWork.Repository<Receptionist>().ListAsync(specification);
            int totalCount = await _unitOfWork.Repository<Receptionist>().CountAsync(specification);
            return new PagedResponse<ReceptionistResponseDto>
            {
                Items = receptionists.Select(ToDto).ToList(),
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                TotalCount = totalCount
            };
        }


        //Helper Methods

        private ReceptionistResponseDto ToDto(Receptionist receptionist)
        {
            return new ReceptionistResponseDto
            {
                Email = receptionist.User.Email,
                FullName = receptionist.User.FullName,
                Id = receptionist.Id,
                UserId = receptionist.User.Id,
                PhoneNumber = receptionist.PhoneNumber
            };
        }
    }
}
