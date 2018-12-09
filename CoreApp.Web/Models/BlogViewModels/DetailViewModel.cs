using CoreApp.Application.ViewModels;
using System.Collections.Generic;

namespace CoreApp.Web.Models.BlogViewModels
{
    public class DetailViewModel
    {
        public BlogViewModel Blog { get; set; }

        public List<BlogViewModel> RelatedBlogs { get; set; }

        public List<TagViewModel> Tags { set; get; }
    }
}
