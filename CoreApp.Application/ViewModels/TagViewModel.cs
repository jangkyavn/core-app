using System.Collections.Generic;

namespace CoreApp.Application.ViewModels
{
    public class TagViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public virtual ICollection<BlogTagViewModel> BlogTags { get; set; }
        public virtual ICollection<ProductTagViewModel> ProductTags { get; set; }
    }
}
