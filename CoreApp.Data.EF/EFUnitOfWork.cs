using CoreApp.Infrastructure.Interfaces;

namespace CoreApp.Data.EF
{
    public class EFUnitOfWork : IUnitOfWork
    {
        readonly AppDbContext _context;

        public EFUnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
