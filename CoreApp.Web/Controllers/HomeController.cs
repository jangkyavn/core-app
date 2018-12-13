using CoreApp.Application.Interfaces;
using CoreApp.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CoreApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IBlogService _blogService;
        private readonly ICommonService _commonService;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IMemoryCache _cache;

        public HomeController(IProductService productService,
            IBlogService blogService, 
            ICommonService commonService,
            IProductCategoryService productCategoryService,
            IStringLocalizer<HomeController> localizer,
            IMemoryCache memoryCache)
        {
            _blogService = blogService;
            _commonService = commonService;
            _productService = productService;
            _productCategoryService = productCategoryService;
            _localizer = localizer;
            _cache = memoryCache;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["BodyClass"] = "cms-index-index cms-home-page";

            var homeVm = new HomeViewModel();
            homeVm.Title = _localizer["TITLE"];
            homeVm.HomeCategories = await _productCategoryService.GetHomeCategoriesAsync();
            homeVm.PromotionProducts = await _productService.GetPromotionProducts(5);
            homeVm.LastestBlogs = _blogService.GetLastest(5);
            homeVm.HomeSlides = _commonService.GetSlides("top");

            return View(homeVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
