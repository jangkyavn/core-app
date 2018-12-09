using CoreApp.Infrastructure.SharedKernel;

namespace CoreApp.Data.Entities
{
    public class ProductImage : DomainEntity<int>
    {
        public string Caption { get; set; }
        public string Path { get; set; }
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
