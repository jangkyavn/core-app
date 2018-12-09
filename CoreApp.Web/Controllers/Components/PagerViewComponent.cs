using CoreApp.Utilities.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreApp.Web.Controllers.Components
{
    public class PagerViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(PagedResultBase pagedResult)
        {
            return await Task.FromResult((IViewComponentResult)View("Default", pagedResult));
        }
    }
}
