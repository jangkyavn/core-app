using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IAuthorizationService _authorizationService;

        public RoleController(IRoleService roleService,
            IAuthorizationService authorizationService)
        {
            _roleService = roleService;
            _authorizationService = authorizationService;
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