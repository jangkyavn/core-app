using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;

namespace CoreApp.Data.EF.Repositories
{
    public class AnnouncementUserRepository : EFRepository<AnnouncementUser, int>, IAnnouncementUserRepository
    {
        public AnnouncementUserRepository(AppDbContext context) : base(context)
        {
        }
    }
}
