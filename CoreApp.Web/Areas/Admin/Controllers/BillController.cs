using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Enums;
using CoreApp.Utilities.Constants;
using CoreApp.Utilities.Extensions;
using CoreApp.Utilities.Helpers;
using CoreApp.Web.Areas.Admin.Models;
using CoreApp.Web.Authorization;
using CoreApp.Web.Extensions;
using CoreApp.Web.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class BillController : BaseController
    {
        private readonly IBillService _billService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAnnouncementService _announcementService;
        private readonly IHubContext<CoreHub, ICoreHub> _hubContext;

        public BillController(IBillService billService,
            IHostingEnvironment hostingEnvironment,
            IAuthorizationService authorizationService,
            IAnnouncementService announcementService,
            IHubContext<CoreHub, ICoreHub> hubContext)
        {
            _billService = billService;
            _hostingEnvironment = hostingEnvironment;
            _authorizationService = authorizationService;
            _announcementService = announcementService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BILL", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            return View();
        }

        #region Ajax API
        [HttpGet]
        public async Task<IActionResult> GetById(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BILL", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (id == null)
            {
                return new BadRequestResult();
            }

            var model = _billService.GetDetail(id.Value);
            if (model == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaging(string startDate, string endDate, string keyword, int page, int pageSize)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BILL", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = _billService.GetAllPaging(startDate, endDate, keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentMethod()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BILL", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            List<EnumModel> enums = ((PaymentMethod[])Enum.GetValues(typeof(PaymentMethod)))
                .Select(c => new EnumModel()
                {
                    Value = (int)c,
                    Name = c.GetDescription()
                }).ToList();
            return new OkObjectResult(enums);
        }

        [HttpGet]
        public async Task<IActionResult> GetBillStatus()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BILL", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            List<EnumModel> enums = ((BillStatus[])Enum.GetValues(typeof(BillStatus)))
                .Select(c => new EnumModel()
                {
                    Value = (int)c,
                    Name = c.GetDescription()
                }).ToList();
            return new OkObjectResult(enums);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEntity(BillViewModel billVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            var isAddNew = true;
            if (billVm.Id == 0)
            {
                var result = await _authorizationService.AuthorizeAsync(User, "BILL", Operations.Create);
                if (result.Succeeded == false)
                    return StatusCode(401);

                var announcementViewModel = new AnnouncementViewModel()
                {
                    Content = $"Thêm mới hóa đơn của người dùng {billVm.CustomerName}",
                    DateCreated = DateTime.Now,
                    Status = Status.Active,
                    Title = "Thêm mới",
                    UserId = User.GetUserId(),
                    FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                    Id = Guid.NewGuid().ToString()
                };
                await _announcementService.AddAsync(announcementViewModel);
                await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                _billService.Create(billVm);
            }
            else
            {
                var result = await _authorizationService.AuthorizeAsync(User, "BILL", Operations.Update);
                if (result.Succeeded == false)
                    return StatusCode(401);

                var announcementViewModel = new AnnouncementViewModel()
                {
                    Content = $"Cập nhật hóa đơn của người dùng {billVm.CustomerName}",
                    DateCreated = DateTime.Now,
                    Status = Status.Active,
                    Title = "Cập nhật",
                    UserId = User.GetUserId(),
                    FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                    Id = Guid.NewGuid().ToString()
                };
                await _announcementService.AddAsync(announcementViewModel);
                await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                _billService.Update(billVm);
                isAddNew = false;
            }

            _billService.Save();
            return new OkObjectResult(isAddNew);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int billId, BillStatus status)
        {
            _billService.UpdateStatus(billId, status);

            return new OkObjectResult(true);
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel(int billId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "BILL", Operations.Create);
            if (result.Succeeded == false)
                return StatusCode(401);

            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = $"Bill_{billId}.xlsx";
            // Template File
            string templateDocument = Path.Combine(sWebRootFolder, "templates", "BillTemplate.xlsx");

            string url = $"{Request.Scheme}://{Request.Host}/{"export-files"}/{sFileName}";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, "export-files", sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }

            using (FileStream templateDocumentStream = System.IO.File.OpenRead(templateDocument))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    // add a new worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["TEDUOrder"];
                    // Data Acces, load order header data.
                    var billDetail = _billService.GetDetail(billId);

                    // Insert customer data into template
                    worksheet.Cells[4, 1].Value = "Tên khách hàng: " + billDetail.CustomerName;
                    worksheet.Cells[5, 1].Value = "Địa chỉ: " + billDetail.CustomerAddress;
                    worksheet.Cells[6, 1].Value = "Số điện thoại: " + billDetail.CustomerMobile;
                    // Start Row for Detail Rows
                    int rowIndex = 9;

                    // load order details
                    var orderDetails = _billService.GetBillDetails(billId);
                    int count = 1;
                    foreach (var orderDetail in orderDetails)
                    {
                        // Cell 1, Carton Count
                        worksheet.Cells[rowIndex, 1].Value = count.ToString();
                        // Cell 2, Order Number (Outline around columns 2-7 make it look like 1 column)
                        worksheet.Cells[rowIndex, 2].Value = orderDetail.Product.Name;
                        // Cell 8, Weight in LBS (convert KG to LBS, and rounding to whole number)
                        worksheet.Cells[rowIndex, 3].Value = orderDetail.Quantity.ToString();

                        worksheet.Cells[rowIndex, 4].Value = orderDetail.Price.ToString("N0");
                        worksheet.Cells[rowIndex, 5].Value = (orderDetail.Price * orderDetail.Quantity).ToString("N0");
                        // Increment Row Counter
                        rowIndex++;
                        count++;
                    }
                    decimal total = (decimal)(orderDetails.Sum(x => x.Quantity * x.Price));
                    worksheet.Cells[24, 5].Value = total.ToString("N0");

                    var numberWord = "Tổng tiền (viết bằng chữ): " + TextHelper.ToString(total);
                    worksheet.Cells[26, 1].Value = numberWord;
                    var billDate = billDetail.DateCreated;
                    worksheet.Cells[28, 3].Value = billDate.Value.Day + ", " + billDate.Value.Month + ", " + billDate.Value.Year;

                    package.SaveAs(file); //Save the workbook.
                }
            }
            return new OkObjectResult(url);
        }
        #endregion
    }
}