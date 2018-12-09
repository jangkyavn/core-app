using CoreApp.Application.Interfaces;
using CoreApp.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CoreApp.Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ITagService _tagService;
        private readonly IConfiguration _configuration;

        public BlogController(IBlogService blogService,
            ITagService tagService,
            IConfiguration configuration)
        {
            _blogService = blogService;
            _tagService = tagService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? pageSize, int page = 1)
        {
            ViewData["BodyClass"] = "blog_fullwidth_page";
            if (pageSize == null)
                pageSize = _configuration.GetValue<int>("PageSize");

            var data = await _blogService.GetAllPagingAsync(string.Empty, pageSize.Value, page);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> BlogTag(string tagId, int? pageSize, int page = 1)
        {
            var blogs = new BlogsByTagViewModel();
            ViewData["BodyClass"] = "blog_fullwidth_page";
            if (pageSize == null)
                pageSize = _configuration.GetValue<int>("PageSize");

            var data = _blogService.GetBlogsPagingByTag(tagId, page, pageSize.Value);
            var tag = await _tagService.GetByIdAsync(tagId);

            blogs.Data = data;
            blogs.TagName = tag.Name;

            return View(blogs);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            var detail = new DetailViewModel();
            ViewData["BodyClass"] = "single_post_page";

            if (id == null)
            {
                return new BadRequestResult();
            }

            var viewModel = await _blogService.GetByIdAsync(id.Value);
            detail.Blog = viewModel;
            detail.RelatedBlogs = await _blogService.GetReatedBlogsAsync(id.Value, 4);
            detail.Tags = await _blogService.GetTagsByBlogIdAsync(id.Value);

            return View(detail);
        }
    }
}