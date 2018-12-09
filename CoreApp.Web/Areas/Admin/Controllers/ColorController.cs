using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class ColorController : BaseController
    {
        private readonly IColorService _colorService;
        private readonly IAuthorizationService _authorizationService;

        public ColorController(IColorService colorService,
            IAuthorizationService authorizationService)
        {
            _colorService = colorService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "COLOR", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            return View();
        }

        #region Ajax API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "COLOR", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _colorService.GetAllAsync();
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "COLOR", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _colorService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "COLOR", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (id == null)
            {
                return new BadRequestResult();
            }

            var model = await _colorService.GetById(id.Value);
            return new OkObjectResult(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEntity(ColorViewModel colorViewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            var isAddNew = true;
            if (colorViewModel.Id == 0)
            {
                var result = await _authorizationService.AuthorizeAsync(User, "COLOR", Operations.Create);
                if (result.Succeeded == false)
                    return StatusCode(401);

                _colorService.Add(colorViewModel);
            }
            else
            {
                var result = await _authorizationService.AuthorizeAsync(User, "COLOR", Operations.Update);
                if (result.Succeeded == false)
                    return StatusCode(401);

                _colorService.Update(colorViewModel);
                isAddNew = false;
            }

            _colorService.Save();
            return new OkObjectResult(isAddNew);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "COLOR", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (id == null || id == 0)
            {
                return new BadRequestResult();
            }

            _colorService.Delete(id.Value);
            _colorService.Save();

            return new OkObjectResult(id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple(string jsonId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "COLOR", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (string.IsNullOrEmpty(jsonId))
            {
                return new BadRequestResult();
            }

            var ids = JsonConvert.DeserializeObject<List<int>>(jsonId);
            _colorService.DeleteMultiple(ids);
            _colorService.Save();

            return new OkObjectResult(true);
        }
        #endregion
    }
}