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
    public class ProductCategoryController : BaseController
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAnnouncementService _announcementService;
        private readonly IHubContext<CoreHub, ICoreHub> _hubContext;

        public ProductCategoryController(IProductCategoryService productCategoryService,
            IAuthorizationService authorizationService,
            IAnnouncementService announcementService,
            IHubContext<CoreHub, ICoreHub> hubContext)
        {
            _productCategoryService = productCategoryService;
            _authorizationService = authorizationService;
            _announcementService = announcementService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_CATEGORY", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            return View();
        }

        #region Ajax API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_CATEGORY", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _productCategoryService.GetAllAsync();
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllParent()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_CATEGORY", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _productCategoryService.GetAllParentAsync();
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHierarchy()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_CATEGORY", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _productCategoryService.GetAllHierarchyAsync();
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_CATEGORY", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (id == null)
            {
                return new BadRequestResult();
            }

            var model = _productCategoryService.GetById(id.Value);
            return new OkObjectResult(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEntity(ProductCategoryViewModel viewModel)
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
                    var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_CATEGORY", Operations.Create);
                    if (result.Succeeded == false)
                        return StatusCode(401);

                    var announcementViewModel = new AnnouncementViewModel()
                    {
                        Content = $"Thêm mới danh mục sản phẩm có tên {viewModel.Name}",
                        DateCreated = DateTime.Now,
                        Status = Status.Active,
                        Title = "Thêm mới",
                        UserId = User.GetUserId(),
                        FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                        Id = Guid.NewGuid().ToString()
                    };
                    await _announcementService.AddAsync(announcementViewModel);
                    await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                    _productCategoryService.Add(viewModel);
                }
                else
                {
                    var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_CATEGORY", Operations.Update);
                    if (result.Succeeded == false)
                        return StatusCode(401);

                    var announcementViewModel = new AnnouncementViewModel()
                    {
                        Content = $"Cập nhật danh mục sản phẩm có tên {viewModel.Name}",
                        DateCreated = DateTime.Now,
                        Status = Status.Active,
                        Title = "Cập nhật",
                        UserId = User.GetUserId(),
                        FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                        Id = Guid.NewGuid().ToString()
                    };
                    await _announcementService.AddAsync(announcementViewModel);
                    await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                    _productCategoryService.Update(viewModel);
                    isAddNew = false;
                }

                _productCategoryService.Save();
                return new OkObjectResult(isAddNew);

            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTreeNodePosition(string jsonModel)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_CATEGORY", Operations.Update);
            if (result.Succeeded == false)
                return StatusCode(401);

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Cập nhật vị chí danh mục sản phẩm",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Cập nhật",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            await _productCategoryService.UpdateTreeNodePosition(jsonModel);
            _productCategoryService.Save();

            return new OkObjectResult(true);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_CATEGORY", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (id == null)
            {
                return new BadRequestResult();
            }

            var viewModel = _productCategoryService.GetById(id.Value);
            if (viewModel == null)
            {
                return new NotFoundResult();
            }

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Xóa danh mục sản phẩm có tên {viewModel.Name}",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Xóa",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            _productCategoryService.Delete(id.Value);
            _productCategoryService.Save();

            return new OkObjectResult(id);
        }
        #endregion
    }
}