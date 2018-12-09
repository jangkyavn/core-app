using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;

namespace CoreApp.Data.EF.Repositories
{
    public class ContactDetailRepository : EFRepository<ContactDetail, string>, IContactDetailRepository
    {
        public ContactDetailRepository(AppDbContext context) : base(context)
        {
        }
    }
}
