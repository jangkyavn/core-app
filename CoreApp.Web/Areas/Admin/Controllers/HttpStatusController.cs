using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class HttpStatusController : BaseController
    {
        [HttpGet("statuscode/{code}")]
        public IActionResult Index(HttpStatusCode code)
        {
            return View(code);
        }
    }
}