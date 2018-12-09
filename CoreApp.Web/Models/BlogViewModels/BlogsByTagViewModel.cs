using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Dtos;

namespace CoreApp.Web.Models.BlogViewModels
{
    public class BlogsByTagViewModel
    {
        public PagedResult<BlogViewModel> Data { get; set; }

        public string TagName { get; set; }
    }
}
