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
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAnnouncementService _announcementService;
        private readonly IHubContext<CoreHub, ICoreHub> _hubContext;

        public UserController(IUserService userService,
            IAuthorizationService authorizationService,
            IAnnouncementService announcementService,
            IHubContext<CoreHub, ICoreHub> hubContext)
        {
            _userService = userService;
            _authorizationService = authorizationService;
            _announcementService = announcementService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "USER", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            return View();
        }

        #region Ajax API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "USER", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _userService.GetAllAsync();

            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "USER", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _userService.GetByIdAsync(id);

            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "USER", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _userService.GetAllPagingAsync(keyword, page, pageSize);

            return new OkObjectResult(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEntity(AppUserViewModel userVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var isAddNew = true;

                if (userVm.Id == null)
                {
                    var result = await _authorizationService.AuthorizeAsync(User, "USER", Operations.Create);
                    if (result.Succeeded == false)
                        return StatusCode(401);

                    var announcementViewModel = new AnnouncementViewModel()
                    {
                        Content = $"Thêm mới người dùng có email {userVm.Email}",
                        DateCreated = DateTime.Now,
                        Status = Status.Active,
                        Title = "Thêm mới",
                        UserId = User.GetUserId(),
                        FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                        Id = Guid.NewGuid().ToString()
                    };
                    await _announcementService.AddAsync(announcementViewModel);
                    await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                    await _userService.AddAsync(userVm);
                }
                else
                {
                    var result = await _authorizationService.AuthorizeAsync(User, "USER", Operations.Update);
                    if (result.Succeeded == false)
                        return StatusCode(401);

                    var announcementViewModel = new AnnouncementViewModel()
                    {
                        Content = $"Cập nhật người dùng có email {userVm.Email}",
                        DateCreated = DateTime.Now,
                        Status = Status.Active,
                        Title = "Cập nhật",
                        UserId = User.GetUserId(),
                        FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                        Id = Guid.NewGuid().ToString()
                    };
                    await _announcementService.AddAsync(announcementViewModel);
                    await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                    await _userService.UpdateAsync(userVm);
                    isAddNew = false;
                }

                return new OkObjectResult(isAddNew);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "USER", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (string.IsNullOrEmpty(id.ToString()))
            {
                return new BadRequestResult();
            }

            var viewModel = await _userService.GetByIdAsync(id.Value);
            if (viewModel == null)
            {
                return new NotFoundResult();
            }

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Xóa người dùng có email {viewModel.Email}",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Xóa",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            await _userService.DeleteAsync(id.Value);
            return new OkObjectResult(id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple(string jsonId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "USER", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (string.IsNullOrEmpty(jsonId))
            {
                return new BadRequestResult();
            }

            var listId = JsonConvert.DeserializeObject<List<Guid>>(jsonId);
            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Xóa {listId.Count} người dùng",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Xóa nhiều",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            await _userService.DeleteMultiAsync(listId);
            return new OkObjectResult(true);
        }
        #endregion
    }
}