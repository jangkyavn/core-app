namespace CoreApp.Application.ViewModels
{
    public class BlogTagViewModel
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string TagId { get; set; }

        public virtual BlogViewModel Blog { get; set; }
        public virtual TagViewModel Tag { get; set; }
    }
}
