using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Enums;
using CoreApp.Utilities.Constants;
using CoreApp.Web.Authorization;
using CoreApp.Web.Extensions;
using CoreApp.Web.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IAnnouncementService _announcementService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHubContext<CoreHub, ICoreHub> _hubContext;

        public RoleController(IRoleService roleService,
            IAnnouncementService announcementService,
            IAuthorizationService authorizationService,
            IHubContext<CoreHub, ICoreHub> hubContext)
        {
            _roleService = roleService;
            _announcementService = announcementService;
            _authorizationService = authorizationService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "ROLE", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            return View();
        }

        #region Ajax API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "ROLE", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _roleService.GetAllAsync();
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "ROLE", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (id == null)
            {
                return new BadRequestResult();
            }

            var model = await _roleService.GetById(id.Value);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "ROLE", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _roleService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetListFunctionWithRole(Guid roleId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "ROLE", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var functions = await _roleService.GetListFunctionWithRoleAsync(roleId);
            return new OkObjectResult(functions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEntity(AppRoleViewModel roleVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            var isAddNew = true;
            if (!roleVm.Id.HasValue)
            {
                var result = await _authorizationService.AuthorizeAsync(User, "ROLE", Operations.Create);
                if (result.Succeeded == false)
                    return StatusCode(401);

                await _roleService.AddAsync(roleVm);
            }
            else
            {
                var result = await _authorizationService.AuthorizeAsync(User, "ROLE", Operations.Update);
                if (result.Succeeded == false)
                    return StatusCode(401);

                var announcementViewModel = new AnnouncementViewModel()
                {
                    Content = $"Cập nhật vai trò có tên là: {roleVm.Name}",
                    DateCreated = DateTime.Now,
                    Status = Status.Active,
                    Title = "Cập nhật",
                    UserId = User.GetUserId(),
                    FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                    Id = Guid.NewGuid().ToString()
                };
                await _announcementService.AddAsync(announcementViewModel);
                await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                await _roleService.UpdateAsync(roleVm);
                isAddNew = false;
            }

            return new OkObjectResult(isAddNew);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "ROLE", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _roleService.DeleteAsync(id);
            return new OkObjectResult(id);
        }

        [HttpPost]
        public async Task<IActionResult> SavePermission(List<PermissionViewModel> listPermmission, Guid roleId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "ROLE", Operations.Update);
            if (result.Succeeded == false)
                return StatusCode(401);

            _roleService.SavePermission(listPermmission, roleId);
            return new OkObjectResult(true);
        }
        #endregion
    }
}