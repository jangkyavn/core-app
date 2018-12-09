using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class SizeController : BaseController
    {
        private readonly ISizeService _sizeService;
        private readonly IAuthorizationService _authorizationService;

        public SizeController(ISizeService sizeService,
            IAuthorizationService authorizationService)
        {
            _sizeService = sizeService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "SIZE", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            return View();
        }

        #region Ajax API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "SIZE", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _sizeService.GetAllAsync();
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "SIZE", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _sizeService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "SIZE", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (id == null)
            {
                return new BadRequestResult();
            }

            var model = await _sizeService.GetById(id.Value);
            return new OkObjectResult(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEntity(SizeViewModel sizeViewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            var isAddNew = true;
            if (sizeViewModel.Id == 0)
            {
                var result = await _authorizationService.AuthorizeAsync(User, "SIZE", Operations.Create);
                if (result.Succeeded == false)
                    return StatusCode(401);

                _sizeService.Add(sizeViewModel);
            }
            else
            {
                var result = await _authorizationService.AuthorizeAsync(User, "SIZE", Operations.Update);
                if (result.Succeeded == false)
                    return StatusCode(401);

                _sizeService.Update(sizeViewModel);
                isAddNew = false;
            }

            _sizeService.Save();
            return new OkObjectResult(isAddNew);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "SIZE", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (id == null || id == 0)
            {
                return new BadRequestResult();
            }

            _sizeService.Delete(id.Value);
            _sizeService.Save();

            return new OkObjectResult(id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple(string jsonId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "SIZE", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (string.IsNullOrEmpty(jsonId))
            {
                return new BadRequestResult();
            }

            var ids = JsonConvert.DeserializeObject<List<int>>(jsonId);
            _sizeService.DeleteMultiple(ids);
            _sizeService.Save();

            return new OkObjectResult(true);
        }
        #endregion
    }
}