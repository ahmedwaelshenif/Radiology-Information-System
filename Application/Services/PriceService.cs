using Application.Interfaces.IRepositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class PriceService : IPriceService
    {

        private readonly IUnitOfWork _unitOfWork;
        public PriceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task UpdateModalityPrice(Modality mod, decimal amount)
        {
            Price ?entity = await _unitOfWork.Repository<Price>().FirstOrDefaultAsync(p => p.Name == mod.ToString());
            if (entity == null)
            {
                throw new ArgumentException("this modality can not be found");
            }
            entity.Amount = amount;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
