using CoreApp.Infrastructure.SharedKernel;

namespace CoreApp.Data.Entities
{
    public class BillDetail : DomainEntity<int>
    {
        public int BillId { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public virtual Bill Bill { get; set; }
        public virtual Color Color { get; set; }
        public virtual Size Size { get; set; }
        public virtual Product Product { get; set; }
    }
}
