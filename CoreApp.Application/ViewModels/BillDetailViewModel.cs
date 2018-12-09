namespace CoreApp.Application.ViewModels
{
    public class BillDetailViewModel
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public BillViewModel Bill { get; set; }
        public ColorViewModel Color { get; set; }
        public SizeViewModel Size { get; set; }
        public ProductViewModel Product { get; set; }
    }
}
