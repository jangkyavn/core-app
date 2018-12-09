using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Infrastructure.Enums;
using CoreApp.Utilities.Extensions;
using CoreApp.Web.Areas.Admin.Models;
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
    public class FunctionController : BaseController
    {
        private readonly IFunctionService _functionService;
        private readonly IAuthorizationService _authorizationService;

        public FunctionController(
            IFunctionService functionService,
            IAuthorizationService authorizationService)
        {
            _functionService = functionService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FUNCTION", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            return View();
        }

        #region Ajax Request
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FUNCTION", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _functionService.GetAllAsync();
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllParent()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FUNCTION", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _functionService.GetAllParentAsync();
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHierarchy()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FUNCTION", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _functionService.GetAllHierarchyAsync();
            return new ObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FUNCTION", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model =  _functionService.GetById(id);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetIcons()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FUNCTION", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            List<EnumModel> enums = ((FeatherIcons[])Enum.GetValues(typeof(FeatherIcons)))
                .Select(c => new EnumModel()
                {
                    Value = c.ToString(),
                    Name = c.GetDescription()
                }).ToList();
            return new OkObjectResult(enums);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEntity(FunctionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (viewModel.IsAddNew)
            {
                var result = await _authorizationService.AuthorizeAsync(User, "FUNCTION", Operations.Create);
                if (result.Succeeded == false)
                    return StatusCode(401);

                _functionService.Add(viewModel);
            }
            else
            {
                var result = await _authorizationService.AuthorizeAsync(User, "FUNCTION", Operations.Update);
                if (result.Succeeded == false)
                    return StatusCode(401);

                _functionService.Update(viewModel);
            }

            _functionService.Save();

            return new OkObjectResult(true);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTreeNodePosition(string jsonModel)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FUNCTION", Operations.Update);
            if (result.Succeeded == false)
                return StatusCode(401);

            await _functionService.UpdateTreeNodePosition(jsonModel);
            _functionService.Save();

            return new OkObjectResult(true);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FUNCTION", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestResult();
            }

            _functionService.Delete(id);
            _functionService.Save();

            return new OkObjectResult(true);
        }
        #endregion
    }
}