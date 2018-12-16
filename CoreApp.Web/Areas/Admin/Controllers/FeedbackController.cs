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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class FeedbackController : BaseController
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAnnouncementService _announcementService;
        private readonly IHubContext<CoreHub, ICoreHub> _hubContext;

        public FeedbackController(IFeedbackService feedbackService,
            IAuthorizationService authorizationService,
            IAnnouncementService announcementService,
            IHubContext<CoreHub, ICoreHub> hubContext)
        {
            _feedbackService = feedbackService;
            _authorizationService = authorizationService;
            _announcementService = announcementService;
            _hubContext = hubContext;
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

                var announcementViewModel = new AnnouncementViewModel()
                {
                    Content = $"Thêm mới phản hồi có email {feedbackViewModel.Email}",
                    DateCreated = DateTime.Now,
                    Status = Status.Active,
                    Title = "Thêm mới",
                    UserId = User.GetUserId(),
                    FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                    Id = Guid.NewGuid().ToString()
                };
                await _announcementService.AddAsync(announcementViewModel);
                await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                _feedbackService.Add(feedbackViewModel);
            }
            else
            {
                var result = await _authorizationService.AuthorizeAsync(User, "FEEDBACK", Operations.Update);
                if (result.Succeeded == false)
                    return StatusCode(401);

                var announcementViewModel = new AnnouncementViewModel()
                {
                    Content = $"Cập nhật phản hồi có email {feedbackViewModel.Email}",
                    DateCreated = DateTime.Now,
                    Status = Status.Active,
                    Title = "Cập nhật",
                    UserId = User.GetUserId(),
                    FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                    Id = Guid.NewGuid().ToString()
                };
                await _announcementService.AddAsync(announcementViewModel);
                await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

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

            if (id == null)
            {
                return new BadRequestResult();
            }

            var viewModel = await _feedbackService.GetByIdAsync(id.Value);
            if (viewModel == null)
            {
                return new NotFoundResult();
            }

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Xóa phản hồi có email {viewModel.Email}",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Xóa",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

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

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Xóa phản {ids.Count} phản hồi",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Xóa nhiều",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            await _feedbackService.DeleteMultipleAsync(ids);
            _feedbackService.SaveChanges();

            return new OkObjectResult(true);
        }
        #endregion
    }
}