using CoreApp.Infrastructure.SharedKernel;

namespace CoreApp.Data.Entities
{
    public class ProductQuantity : DomainEntity<int>
    {
        public int ColorId { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }

        public virtual Color Color { get; set; }
        public virtual Product Product { get; set; }
        public virtual Size Size { get; set; }
    }
}
