using CoreApp.Application.Interfaces;
using CoreApp.Web.Models.ProductViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IBillService _billService;
        private readonly IColorService _colorService;
        private readonly ISizeService _sizeService;
        private readonly ITagService _tagService;
        private readonly IReviewService _reviewService;
        private readonly IConfiguration _configuration;

        public ProductController(IProductService productService,
            IProductCategoryService productCategoryService,
            IBillService billService,
            IColorService colorService,
            ISizeService sizeService,
            ITagService tagService,
            IReviewService reviewService,
            IConfiguration configuration)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _billService = billService;
            _colorService = colorService;
            _sizeService = sizeService;
            _tagService = tagService;
            _reviewService = reviewService;
            _configuration = configuration;
        }

        public IActionResult Catalog(int id, int? pageSize, string sortBy, int page = 1)
        {
            var catalog = new CatalogViewModel();
            ViewData["BodyClass"] = "shop_grid_page";
            if (pageSize == null)
                pageSize = _configuration.GetValue<int>("PageSize");

            catalog.PageSize = pageSize;
            catalog.SortType = sortBy;
            catalog.Data = _productService.GetAllPaging(string.Empty, id, sortBy, page, pageSize.Value);
            catalog.Category = _productCategoryService.GetById(id);
            catalog.Breadcrumbs = _productCategoryService.GetBreadcrumbs(id);

            return View(catalog);
        }

        public IActionResult Search(string keyword, int? pageSize, string sortBy, int page = 1)
        {
            var catalog = new SearchResultViewModel();
            ViewData["BodyClass"] = "shop_grid_page";
            if (pageSize == null)
                pageSize = _configuration.GetValue<int>("PageSize");

            catalog.PageSize = pageSize;
            catalog.SortType = sortBy;
            catalog.Data = _productService.GetAllPaging(keyword, null, sortBy, page, pageSize.Value);
            catalog.Keyword = keyword;

            return View(catalog);
        }

        [HttpGet]
        public async Task<IActionResult> ProductTag(string tagId, string sortType, int? pageSize, int page = 1)
        {
            var productsByTag = new ProductsByTagViewModel();
            ViewData["BodyClass"] = "shop_grid_page";
            if (pageSize == null)
                pageSize = _configuration.GetValue<int>("PageSize");

            var products = _productService.GetProductsPagingByTag(tagId, sortType, page, pageSize.Value);
            var tag = await _tagService.GetByIdAsync(tagId);

            productsByTag.SortType = sortType;
            productsByTag.PageSize = pageSize;
            productsByTag.Data = products;
            productsByTag.TagId = tagId;
            productsByTag.TagName = tag.Name;

            return View(productsByTag);
        }

        public async Task<IActionResult> Detail(int id)
        {
            ViewData["BodyClass"] = "product-page";
            var model = new DetailViewModel();
            model.Product = _productService.GetById(id);
            model.RelatedProducts = await _productService.GetRelatedProductsAsync(id, 5);
            model.ProductImages = _productService.GetImages(id);
            model.Reviews = await _reviewService.GetAllAsync();
            model.Tags = _productService.GetProductTags(id);
            model.Breadcrumbs = _productService.GetBreadcrumbs(id);
            model.Rating = await _reviewService.GetRatingAverageAsync(id);
            model.RatingTotal = await _reviewService.GetRatingTotalAsync(id);

            var colors = await _colorService.GetAllAsync();
            var sizes = await _sizeService.GetAllAsync();

            model.Colors = colors.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            model.Sizes = sizes.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            return View(model);
        }

        #region Ajax API
        [HttpGet]
        public async Task<IActionResult> GetQuickViewData(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            var viewModel = _productService.GetById(id.Value);
            var rating = await _reviewService.GetRatingAverageAsync(id.Value);
            var colors = await _colorService.GetAllAsync();
            var sizes = await _sizeService.GetAllAsync();

            return new OkObjectResult(new
            {
                Data = viewModel,
                Rating = rating,
                Colors = colors,
                Sizes = sizes
            });
        }

        [HttpGet]
        public IActionResult SuggestSearchResult(string keyword)
        {
            var model = _productService.SuggestSearchResult(keyword);

            return new OkObjectResult(model);
        }
        #endregion
    }
}