using CoreApp.Data.Entities;
using CoreApp.Infrastructure.Interfaces;

namespace CoreApp.Data.IRepositories
{
    public interface ITagRepository : IRepository<Tag, string>
    {
    }
}
