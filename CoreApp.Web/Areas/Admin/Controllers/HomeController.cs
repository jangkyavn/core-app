using CoreApp.Application.Dapper.Interfaces;
using CoreApp.Application.Interfaces;
using CoreApp.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IBillService _billService;
        private readonly IReportService _reportService;
        private readonly IAuthorizationService _authorizationService;

        public HomeController(
            IUserService userService,
            IProductService productService,
            IBillService billService,
            IReportService reportService,
            IAuthorizationService authorizationService)
        {
            _userService = userService;
            _productService = productService;
            _billService = billService;
            _reportService = reportService;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel();

            model.TotalOrders = await _billService.GetTotalAmount();
            model.TotalProducts = await _productService.GetTotalAmount();
            model.TotalProfit = 0;
            model.TotalUsers = await _userService.GetTotalAmount();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetRevenues(string fromDate, string toDate)
        {
            return new OkObjectResult(await _reportService.GetReportsAsync(fromDate, toDate));
        }
    }
}