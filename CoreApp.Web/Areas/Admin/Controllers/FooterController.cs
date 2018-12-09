using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Constants;
using CoreApp.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class FooterController : BaseController
    {
        private readonly IFooterService _footerService;
        private readonly IAuthorizationService _authorizationService;

        public FooterController(IFooterService footerService,
            IAuthorizationService authorizationService)
        {
            _footerService = footerService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FOOTER", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            return View();
        }

        #region Ajax API
        [HttpGet]
        public async Task<IActionResult> GetFooter()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FOOTER", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var viewModel = await _footerService.GetFooterAsync();
            return new OkObjectResult(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEntity(FooterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            var isAddView = true;
            var hasData = await _footerService.HasDataAsync();
            if (!hasData)
            {
                var result = await _authorizationService.AuthorizeAsync(User, "FOOTER", Operations.Create);
                if (result.Succeeded == false)
                    return StatusCode(401);

                viewModel.Id = CommonConstants.DefaultFooterId;
                _footerService.Add(viewModel);
            }
            else
            {
                var result = await _authorizationService.AuthorizeAsync(User, "FOOTER", Operations.Update);
                if (result.Succeeded == false)
                    return StatusCode(401);

                _footerService.Update(viewModel);
                isAddView = false;
            }

            _footerService.Save();

            return new OkObjectResult(isAddView);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FOOTER", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestResult();
            }

            _footerService.Delete(id);
            _footerService.Save();

            return new OkObjectResult(true);
        }
        #endregion
    }
}