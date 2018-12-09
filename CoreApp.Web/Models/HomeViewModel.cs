using CoreApp.Application.ViewModels;
using System.Collections.Generic;

namespace CoreApp.Web.Models
{
    public class HomeViewModel
    {
        public List<BlogViewModel> LastestBlogs { get; set; }
        public List<SlideViewModel> HomeSlides { get; set; }
        public List<ProductViewModel> PromotionProducts { get; set; }
        public List<ProductCategoryViewModel> HomeCategories { set; get; }

        public string Title { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
    }
}
