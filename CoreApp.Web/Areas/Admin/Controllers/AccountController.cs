using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;
using CoreApp.Data.Enums;
using CoreApp.Utilities.Dtos;
using CoreApp.Web.Areas.Admin.Models;
using CoreApp.Web.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAnnouncementService _announcementService;
        private readonly IHubContext<CoreHub, ICoreHub> _hubContext;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IAnnouncementService announcementService,
            IHubContext<CoreHub, ICoreHub> hubContext,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _announcementService = announcementService;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return Redirect("/admin/home/index");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserName, 
                                                                    model.Password, 
                                                                    model.RememberMe, 
                                                                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);

                    var announcementViewModel = new AnnouncementViewModel()
                    {
                        Content = $"Người dùng {user.FullName} đã đăng nhập",
                        DateCreated = DateTime.Now,
                        Status = Status.Active,
                        Title = "Đăng nhập",
                        UserId = user.Id,
                        FullName = user.FullName,
                        Id = Guid.NewGuid().ToString()
                    };
                    await _announcementService.AddAsync(announcementViewModel);
                    await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                    _logger.LogInformation("User logged in.");
                    return new OkObjectResult(new GenericResult(true));
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return new ObjectResult(new GenericResult(false, "Tài khoản đã bị khoá"));
                }
                else
                {
                    return new ObjectResult(new GenericResult(false, "Tài khoản hoặc mật khẩu không đúng"));
                }
            }

            // If we got this far, something failed, redisplay form
            return new ObjectResult(new GenericResult(false, model));
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return Redirect("/admin");
        }
    }
}