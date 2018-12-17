using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Web.Models.ProductViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Web.Controllers.Components
{
    public class SidebarFilterViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IColorService _colorService;
        private readonly ISizeService _sizeService;
        private readonly ITagService _tagService;

        public SidebarFilterViewComponent(IProductService productService,
            IProductCategoryService productCategoryService,
            IColorService colorService,
            ISizeService sizeService,
            ITagService tagService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _colorService = colorService;
            _sizeService = sizeService;
            _tagService = tagService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int categoryId, decimal? fromPrice, decimal? toPrice)
        {
            var model = new SidebarFilterViewModel();

            var colors = await _colorService.GetAllAsync();
            var sizes = await _sizeService.GetAllAsync();
            var products = _productService.GetAllPaging(string.Empty, categoryId, null, null, null, 1, -1).Results;

            model.Category = _productCategoryService.GetById(categoryId);
            model.Products = _productService.GetHotProduct(2);
            model.ProductCategories = await _productCategoryService.GetChildrenForFilterAsync();
            model.Tags = await _tagService.GetPopularTagsAsync(10);
            model.MinPrice = products.Min(x => x.Price);
            model.MaxPrice = products.Max(x => x.Price);
            model.FromPrice = fromPrice == null ? products.Min(x => x.Price) : fromPrice;
            model.ToPrice = toPrice == null ? products.Max(x => x.Price) : toPrice;

            return View(model);
        }
    }
}
