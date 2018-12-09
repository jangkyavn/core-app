using CoreApp.Application.Interfaces;
using CoreApp.Utilities.Constants;
using CoreApp.Web.Models;
using CoreApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace CoreApp.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly IFeedbackService _feedbackService;
        private readonly IEmailSender _emailSender;
        private readonly IViewRenderService _viewRenderService;
        private readonly IConfiguration _configuration;

        public ContactController(
            IContactService contactSerivce,
            IFeedbackService feedbackService,
            IEmailSender emailSender,
            IViewRenderService viewRenderService,
            IConfiguration configuration)
        {
            _contactService = contactSerivce;
            _feedbackService = feedbackService;
            _emailSender = emailSender;
            _configuration = configuration;
            _viewRenderService = viewRenderService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var contact = _contactService.GetById(CommonConstants.DefaultContactId);
            var model = new ContactPageViewModel { Contact = contact };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactPageViewModel model)
        {
            if (ModelState.IsValid)
            {
                _feedbackService.Add(model.Feedback);
                _feedbackService.SaveChanges();

                try
                {
                    var content = await _viewRenderService.RenderToStringAsync("Contact/_ContactMail", model.Feedback);
                    await _emailSender.SendEmailAsync(_configuration["MailSettings:AdminMail"], "Have new contact feedback", content);
                    ViewData["Success"] = true;
                }
                catch (Exception)
                {
                    ViewData["Success"] = false;
                }
            }

            model.Contact = _contactService.GetById(CommonConstants.DefaultContactId);
            return View("Index", model);
        }
    }
}