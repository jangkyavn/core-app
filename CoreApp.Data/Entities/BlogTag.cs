using CoreApp.Infrastructure.SharedKernel;

namespace CoreApp.Data.Entities
{
    public class BlogTag : DomainEntity<int>
    {
        public int BlogId { get; set; }
        public string TagId { get; set; }

        public virtual Blog Blog { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
