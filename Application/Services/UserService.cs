using Application.DTOs.RequestDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IPasswordHasher<User> _passwordHasher;


        public UserService(IUnitOfWork unitOfWork, IPasswordHasher<User>  passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }


        public async Task ActivateAsync(int id)
        {
            var user=await _unitOfWork.Repository<User>().GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User Not Found");

            if (user.Role == Role.Admin)
                throw new InvalidOperationException("Cannot Activate an Admin");
            if(user.IsActive)
                throw new InvalidOperationException("User is already active");
            user.IsActive = true;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User Not Found");
            if (user.Role == Role.Admin)
                throw new InvalidOperationException("Cannot Deactivate an Admin");
            if (!user.IsActive)
                throw new InvalidOperationException("User is already inactive");
            user.IsActive = false;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> CountPatientsAsync()
        {
            return await _unitOfWork.Repository<User>().CountAsync(u => u.Role == Role.Patient);
        }

        public async Task<int> CountRadiologistsAsync()
        {
            return await _unitOfWork.Repository<User>().CountAsync(u => u.Role == Role.Radiologist);
        }

        public async Task<int> CountReceptionistsAsync()
        {
            return await _unitOfWork.Repository<User>().CountAsync(u => u.Role == Role.Receptionist);
        }


        public async Task UpdateMyInformation(UpdateUserDto dto, int id)
        {
            User? user = await _unitOfWork.Repository<User>().GetByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException("This user cannot be found");
            }

            if (dto.NewPassword != null)
            {
                if (dto.OldPassword == null)
                {
                    throw new ArgumentException("Please enter your old password");
                }

                var result = _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    dto.OldPassword
                );

                if (result == PasswordVerificationResult.Failed)
                {
                    throw new ArgumentException("Enter a correct old password");
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            }

            if (dto.PhoneNumber != null)
            {
                user.PhoneNumber = dto.PhoneNumber;
            }

            if (dto.Name != null)
            {
                user.FullName = dto.Name;
            }

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateUserInformation(UpdateUserDto dto, int id)
        {
            User? user = await _unitOfWork.Repository<User>().GetByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException("This user cannot be found");
            }

            if (dto.NewPassword != null)
            {

                user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            }

            if (dto.PhoneNumber != null)
            {
                user.PhoneNumber = dto.PhoneNumber;
            }

            if (dto.Name != null)
            {
                user.FullName = dto.Name;
            }

            await _unitOfWork.SaveChangesAsync();
        }

        
      
    }
}
