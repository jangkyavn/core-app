namespace CoreApp.Application.ViewModels
{
    public class ProductImageViewModel
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Path { get; set; }
        public int ProductId { get; set; }

        public ProductViewModel Product { get; set; }
    }
}
