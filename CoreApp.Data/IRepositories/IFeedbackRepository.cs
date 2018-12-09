using CoreApp.Data.Entities;
using CoreApp.Infrastructure.Interfaces;

namespace CoreApp.Data.IRepositories
{
    public interface IFeedbackRepository : IRepository<Feedback, int>
    {
    }
}
