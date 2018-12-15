using CoreApp.Application.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CoreApp.Web.Models.ProductViewModels
{
    public class DetailViewModel
    {
        public ProductViewModel Product { get; set; }

        public string Breadcrumbs { get; set; }

        public bool Available { set; get; }

        public int Rating { get; set; }

        public int RatingTotal { get; set; }

        public List<ProductViewModel> RelatedProducts { get; set; }

        public List<ProductImageViewModel> ProductImages { set; get; }

        public List<ReviewViewModel> Reviews { get; set; }

        public List<TagViewModel> Tags { set; get; }

        public List<SelectListItem> Colors { set; get; }

        public List<SelectListItem> Sizes { set; get; }
    }
}
