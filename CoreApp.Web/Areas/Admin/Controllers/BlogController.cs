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
    public class BlogController : BaseController
    {
        private readonly IBlogService _blogService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAnnouncementService _announcementService;
        private readonly IHubContext<CoreHub, ICoreHub> _hubContext;

        public BlogController(IBlogService blogService,
            IAuthorizationService authorizationService,
            IAnnouncementService announcementService,
            IHubContext<CoreHub, ICoreHub> hubContext)
        {
            _blogService = blogService;
            _authorizationService = authorizationService;
            _announcementService = announcementService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BLOG", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            return View();
        }

        #region Ajax API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BLOG", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _blogService.GetAllAsync();
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BLOG", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _blogService.GetAllPagingAsync(keyword, pageSize, page);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BLOG", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (id == null)
            {
                return new BadRequestResult();
            }

            var model = await _blogService.GetByIdAsync(id.Value);
            return new OkObjectResult(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEntity(BlogViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var isAddNew = true;
                if (viewModel.Id == 0)
                {
                    var result = await _authorizationService.AuthorizeAsync(User, "BLOG", Operations.Create);
                    if (result.Succeeded == false)
                        return StatusCode(401);

                    var announcementViewModel = new AnnouncementViewModel()
                    {
                        Content = $"Thêm mới bài viết có tiêu đề {viewModel.Name}",
                        DateCreated = DateTime.Now,
                        Status = Status.Active,
                        Title = "Thêm mới",
                        UserId = User.GetUserId(),
                        FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                        Id = Guid.NewGuid().ToString()
                    };
                    await _announcementService.AddAsync(announcementViewModel);
                    await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                    _blogService.Add(viewModel);
                }
                else
                {
                    var result = await _authorizationService.AuthorizeAsync(User, "BLOG", Operations.Update);
                    if (result.Succeeded == false)
                        return StatusCode(401);

                    var announcementViewModel = new AnnouncementViewModel()
                    {
                        Content = $"Cập nhật bài viết có tiêu đề {viewModel.Name}",
                        DateCreated = DateTime.Now,
                        Status = Status.Active,
                        Title = "Cập nhật",
                        UserId = User.GetUserId(),
                        FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                        Id = Guid.NewGuid().ToString()
                    };
                    await _announcementService.AddAsync(announcementViewModel);
                    await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                    _blogService.Update(viewModel);
                    isAddNew = false;
                }

                _blogService.Save();
                return new OkObjectResult(isAddNew);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BLOG", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (id == null)
            {
                return new BadRequestResult();
            }

            var viewModel = await _blogService.GetByIdAsync(id.Value);
            if (viewModel == null)
            {
                return new NotFoundResult();
            }

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Xóa bài viết có tiêu đề {viewModel.Name}",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Xóa",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            await _blogService.DeleteAsync(id.Value);
            _blogService.Save();
            return new OkObjectResult(id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple(string jsonId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BLOG", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (string.IsNullOrEmpty(jsonId))
            {
                return new BadRequestResult();
            }

            var listId = JsonConvert.DeserializeObject<List<int>>(jsonId);

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Xóa {listId.Count} bài viết.",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Xóa nhiều",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            await _blogService.DeleteMultipleAsync(listId);
            _blogService.Save();
            return new OkObjectResult(true);
        }
        #endregion
    }
}