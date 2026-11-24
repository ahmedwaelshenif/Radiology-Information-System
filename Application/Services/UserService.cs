using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

      
    }
}
