using CoreApp.Application.Interfaces;
using CoreApp.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class AnnouncementController : BaseController
    {
        private readonly IAnnouncementService _announcementService;
        private readonly IAuthorizationService _authorizationService;

        public AnnouncementController(IAnnouncementService announcementService,
            IAuthorizationService authorizationService)
        {
            _announcementService = announcementService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "ANNOUNCEMENT", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            return View();
        }

        #region Ajax API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "ANNOUNCEMENT", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var viewModel = await _announcementService.GetAllAsync();
            return new OkObjectResult(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "ANNOUNCEMENT", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var viewModel = await _announcementService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "ANNOUNCEMENT", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            await _announcementService.DeleteAsync(id);
            _announcementService.SaveChanges();

            return new OkObjectResult(true);
        }
        #endregion
    }
}