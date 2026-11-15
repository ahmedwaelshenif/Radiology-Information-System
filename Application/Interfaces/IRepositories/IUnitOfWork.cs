using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync();
    }
}
