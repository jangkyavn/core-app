namespace CoreApp.Application.ViewModels
{
    public class ProductTagViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string TagId { get; set; }

        public virtual ProductViewModel Product { get; set; }
        public virtual TagViewModel Tag { get; set; }
    }
}
