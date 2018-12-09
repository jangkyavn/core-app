using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;

namespace CoreApp.Data.EF.Repositories
{
    public class FooterRepository : EFRepository<Footer, string>, IFooterRepository
    {
        public FooterRepository(AppDbContext context) : base(context)
        {
        }
    }
}
