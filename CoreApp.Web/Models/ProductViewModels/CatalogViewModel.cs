using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CoreApp.Web.Models.ProductViewModels
{
    public class CatalogViewModel
    {
        public PagedResult<ProductViewModel> Data { get; set; }

        public ProductCategoryViewModel Category { set; get; }

        public string Breadcrumbs { get; set; }

        public string SortType { set; get; }

        public int? PageSize { set; get; }

        public List<SelectListItem> SortTypes { get; } = new List<SelectListItem>
        {
            new SelectListItem(){Value = "latest",Text = "Mới nhất"},
            new SelectListItem(){Value = "oldest",Text = "Cũ nhất"},
            new SelectListItem(){Value = "price-ascending",Text = "Giá tăng dần"},
            new SelectListItem(){Value = "price-descending",Text = "Giá giảm dần"},
            new SelectListItem(){Value = "name-a-z",Text = "Tên a->z"},
            new SelectListItem(){Value = "name-z-a",Text = "Tên z->a"},
        };

        public List<SelectListItem> PageSizes { get; } = new List<SelectListItem>
        {
            new SelectListItem(){Value = "6",Text = "6"},
            new SelectListItem(){Value = "12",Text = "12"},
            new SelectListItem(){Value = "24",Text = "24"},
        };
    }
}
