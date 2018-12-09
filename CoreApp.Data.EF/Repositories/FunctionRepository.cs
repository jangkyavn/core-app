using System;
using System.Linq;
using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;

namespace CoreApp.Data.EF.Repositories
{
    public class FunctionRepository : EFRepository<Function, string>, IFunctionRepository
    {
        private readonly AppDbContext _context;

        public FunctionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Function> GetListFunctionByPermission(Guid userId)
        {
            var query = from f in _context.Functions
                        join p in _context.Permissions on f.Id equals p.FunctionId
                        join r in _context.AppRoles on p.RoleId equals r.Id
                        join ur in _context.UserRoles on r.Id equals ur.RoleId
                        join u in _context.AppUsers on ur.UserId equals u.Id
                        where u.Id == userId && p.CanRead == true
                        select f;

            var parentIds = query.Select(x => x.ParentId).Distinct();
            return query.Union(_context.Functions.Where(f => parentIds.Contains(f.Id)));
        }
    }
}
