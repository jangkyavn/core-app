using CoreApp.Data.Entities;
using CoreApp.Infrastructure.Interfaces;
using System;
using System.Linq;

namespace CoreApp.Data.IRepositories
{
    public interface IFunctionRepository : IRepository<Function, string>
    {
        IQueryable<Function> GetListFunctionByPermission(Guid userId);
    }
}
