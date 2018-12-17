using CoreApp.Application.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CoreApp.Web.Models.ProductViewModels
{
    public class SidebarFilterViewModel
    {
        public ProductCategoryViewModel Category { get; set; }
        public List<ProductCategoryFilterViewModel> ProductCategories { get; set; }
        public List<ProductViewModel> Products { get; set; }
        public List<PopularTagViewModel> Tags { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }
    }
}
