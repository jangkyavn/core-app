using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Constants;
using CoreApp.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreApp.Web.Areas.Admin.Components
{
    public class SideBarMenuViewComponent : ViewComponent
    {
        private readonly IFunctionService _functionService;

        public SideBarMenuViewComponent(IFunctionService functionService)
        {
            _functionService = functionService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<FunctionViewModel> model = null;
            var userId = ((ClaimsPrincipal)User).GetUserId();

            if (User.IsInRole(CommonConstants.UserRoles.AdminRole))
            {
                model =  await _functionService.GetAllAsync();
            }
            else
            {
                model = await _functionService.GetListFunctionByPermissionAsync(userId);
            }

            return View(model);
        }
    }
}
