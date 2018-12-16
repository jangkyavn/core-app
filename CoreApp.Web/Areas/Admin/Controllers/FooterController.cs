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
    public class FooterController : BaseController
    {
        private readonly IFooterService _footerService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAnnouncementService _announcementService;
        private readonly IHubContext<CoreHub, ICoreHub> _hubContext;

        public FooterController(IFooterService footerService,
            IAuthorizationService authorizationService,
            IAnnouncementService announcementService,
            IHubContext<CoreHub, ICoreHub> hubContext)
        {
            _footerService = footerService;
            _authorizationService = authorizationService;
            _announcementService = announcementService;
            _hubContext = hubContext;
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

                var announcementViewModel = new AnnouncementViewModel()
                {
                    Content = $"Thêm mới footer",
                    DateCreated = DateTime.Now,
                    Status = Status.Active,
                    Title = "Thêm mới",
                    UserId = User.GetUserId(),
                    FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                    Id = Guid.NewGuid().ToString()
                };
                await _announcementService.AddAsync(announcementViewModel);
                await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                viewModel.Id = CommonConstants.DefaultFooterId;
                _footerService.Add(viewModel);
            }
            else
            {
                var result = await _authorizationService.AuthorizeAsync(User, "FOOTER", Operations.Update);
                if (result.Succeeded == false)
                    return StatusCode(401);

                var announcementViewModel = new AnnouncementViewModel()
                {
                    Content = $"Cập nhật footer",
                    DateCreated = DateTime.Now,
                    Status = Status.Active,
                    Title = "Cập nhật",
                    UserId = User.GetUserId(),
                    FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                    Id = Guid.NewGuid().ToString()
                };
                await _announcementService.AddAsync(announcementViewModel);
                await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

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

            var viewModel = await _footerService.GetFooterAsync();
            if (viewModel == null)
            {
                return new NotFoundResult();
            }

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Xóa footer",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Xóa",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            _footerService.Delete(id);
            _footerService.Save();

            return new OkObjectResult(true);
        }
        #endregion
    }
}