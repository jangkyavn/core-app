using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Enums;
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
        private readonly IHubContext<CoreHub, ICoreHub> _hubContext;

        public UserController(IUserService userService,
            IAuthorizationService authorizationService,
            IHubContext<CoreHub, ICoreHub> hubContext)
        {
            _userService = userService;
            _authorizationService = authorizationService;
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

                    await _userService.AddAsync(userVm);
                }
                else
                {
                    var result = await _authorizationService.AuthorizeAsync(User, "USER", Operations.Update);
                    if (result.Succeeded == false)
                        return StatusCode(401);

                    await _hubContext.Clients.All.ReceiveMessage(new AnnouncementViewModel()
                    {
                        Content = $"Updated user at: {DateTime.Now}",
                        DateCreated = DateTime.Now,
                        Status = Status.Active,
                        Title = "Updated",
                        UserId = User.GetUserId()
                    });

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

            await _userService.DeleteMultiAsync(listId);
            return new OkObjectResult(true);
        }
        #endregion
    }
}