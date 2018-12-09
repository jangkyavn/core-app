using CoreApp.Application.Interfaces;
using CoreApp.Infrastructure.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace CoreApp.Web.Controllers.Components
{
    public class MobileMenuViewComponent : ViewComponent
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IMemoryCache _cache;

        public MobileMenuViewComponent(IProductCategoryService productCategoryService,
            IMemoryCache memoryCache)
        {
            _productCategoryService = productCategoryService;
            _cache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _cache.GetOrCreateAsync(CacheKeys.ProductCategories, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(2);
                return await _productCategoryService.GetAllAsync();
            });

            return View(categories);
        }
    }
}
