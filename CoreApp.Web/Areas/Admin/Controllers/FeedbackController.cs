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
    public class FeedbackController : BaseController
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IAuthorizationService _authorizationService;

        public FeedbackController(IFeedbackService feedbackService,
            IAuthorizationService authorizationService)
        {
            _feedbackService = feedbackService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FEEDBACK", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            return View();
        }

        #region Ajax API
        [HttpGet]
        public async Task<IActionResult> GetAll()

        {
            var result = await _authorizationService.AuthorizeAsync(User, "FEEDBACK", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var viewModels = await _feedbackService.GetAllAsync();
            return new OkObjectResult(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FEEDBACK", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var viewModels = await _feedbackService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FEEDBACK", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (string.IsNullOrEmpty(id.ToString()))
            {
                return new BadRequestResult();
            }

            var viewModel = await _feedbackService.GetByIdAsync(id.Value);
            return new OkObjectResult(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEntity(FeedbackViewModel feedbackViewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            var isAddView = true;
            if (feedbackViewModel.Id == 0)
            {
                var result = await _authorizationService.AuthorizeAsync(User, "FEEDBACK", Operations.Create);
                if (result.Succeeded == false)
                    return StatusCode(401);

                _feedbackService.Add(feedbackViewModel);
            }
            else
            {
                var result = await _authorizationService.AuthorizeAsync(User, "FEEDBACK", Operations.Update);
                if (result.Succeeded == false)
                    return StatusCode(401);

                _feedbackService.Update(feedbackViewModel);
                isAddView = false;
            }

            _feedbackService.SaveChanges();
            return new OkObjectResult(isAddView);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FEEDBACK", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            await _feedbackService.DeleteAsync(id.Value);
            _feedbackService.SaveChanges();

            return new OkObjectResult(true);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple(string jsonId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "FEEDBACK", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            var ids = JsonConvert.DeserializeObject<List<int>>(jsonId);
            await _feedbackService.DeleteMultipleAsync(ids);
            _feedbackService.SaveChanges();

            return new OkObjectResult(true);
        }
        #endregion
    }
}