using CoreApp.Data.Entities;
using CoreApp.Infrastructure.Interfaces;

namespace CoreApp.Data.IRepositories
{
    public interface IAnnouncementRepository : IRepository<Announcement, string>
    {
    }
}
