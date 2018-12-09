using CoreApp.Application.Interfaces;
using CoreApp.Utilities.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CoreApp.Web.Controllers
{
    public class AboutController : Controller
    {
        private readonly IContactService _contactService;

        public AboutController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var contact = _contactService.GetById(CommonConstants.DefaultContactId);
            return View(contact);
        }
    }
}