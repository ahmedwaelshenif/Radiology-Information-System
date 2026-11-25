using Application.DTOs.RequestDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task DeactivateAsync(int id);
        Task ActivateAsync(int id);
        Task<int> CountRadiologistsAsync();
        Task<int> CountReceptionistsAsync();
        Task<int> CountPatientsAsync();

        Task UpdateUserInformation(UpdateUserDto dto, int id);

        Task UpdateMyInformation(UpdateUserDto dto, int id);
    }
}
