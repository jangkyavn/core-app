using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class SizeController : BaseController
    {
        private readonly ISizeService _sizeService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAnnouncementService _announcementService;
        private readonly IHubContext<CoreHub, ICoreHub> _hubContext;

        public SizeController(ISizeService sizeService,
            IAuthorizationService authorizationService,
            IAnnouncementService announcementService,
            IHubContext<CoreHub, ICoreHub> hubContext)
        {
            _sizeService = sizeService;
            _authorizationService = authorizationService;
            _announcementService = announcementService;
            _hubContext = hubContext;
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

                var announcementViewModel = new AnnouncementViewModel()
                {
                    Content = $"Thêm mới kích cỡ {sizeViewModel.Name}",
                    DateCreated = DateTime.Now,
                    Status = Status.Active,
                    Title = "Thêm mới",
                    UserId = User.GetUserId(),
                    FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                    Id = Guid.NewGuid().ToString()
                };
                await _announcementService.AddAsync(announcementViewModel);
                await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                _sizeService.Add(sizeViewModel);
            }
            else
            {
                var result = await _authorizationService.AuthorizeAsync(User, "SIZE", Operations.Update);
                if (result.Succeeded == false)
                    return StatusCode(401);

                var announcementViewModel = new AnnouncementViewModel()
                {
                    Content = $"Cập nhật kích cỡ {sizeViewModel.Name}",
                    DateCreated = DateTime.Now,
                    Status = Status.Active,
                    Title = "Cập nhật",
                    UserId = User.GetUserId(),
                    FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                    Id = Guid.NewGuid().ToString()
                };
                await _announcementService.AddAsync(announcementViewModel);
                await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

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

            var viewModel = await _sizeService.GetById(id.Value);
            if (viewModel == null)
            {
                return new NotFoundResult();
            }

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Xóa kích cỡ {viewModel.Name}",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Xóa",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

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

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Xóa {ids.Count} kích cỡ",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Xóa nhiều",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            _sizeService.DeleteMultiple(ids);
            _sizeService.Save();

            return new OkObjectResult(true);
        }
        #endregion
    }
}