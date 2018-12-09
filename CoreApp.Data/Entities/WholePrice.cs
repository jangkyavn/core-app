using CoreApp.Infrastructure.SharedKernel;

namespace CoreApp.Data.Entities
{
    public class WholePrice : DomainEntity<int>
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int FromQuantity { get; set; }
        public int ToQuantity { get; set; }

        public virtual Product Product { get; set; }
    }
}
