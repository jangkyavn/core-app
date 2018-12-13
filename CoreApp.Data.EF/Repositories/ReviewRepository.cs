using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;

namespace CoreApp.Data.EF.Repositories
{
    public class ReviewRepository : EFRepository<Review, int>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }
    }
}
