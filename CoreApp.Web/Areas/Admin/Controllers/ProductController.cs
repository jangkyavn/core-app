using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Enums;
using CoreApp.Utilities.Constants;
using CoreApp.Web.Authorization;
using CoreApp.Web.Extensions;
using CoreApp.Web.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CoreApp.Web.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IAnnouncementService _announcementService;
        private readonly IHubContext<CoreHub, ICoreHub> _hubContext;

        public ProductController(IProductService productService,
            IAuthorizationService authorizationService,
            IHostingEnvironment hostingEnvironment,
            IAnnouncementService announcementService,
            IHubContext<CoreHub, ICoreHub> hubContext)
        {
            _productService = productService;
            _authorizationService = authorizationService;
            _hostingEnvironment = hostingEnvironment;
            _announcementService = announcementService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            return View();
        }

        #region Ajax API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = await _productService.GetAllAsync();
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaging(string keyword, int? categoryId, int page, int pageSize)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var model = _productService.GetAllPaging(keyword, categoryId, null, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (id == null)
            {
                return new BadRequestResult();
            }

            var model = _productService.GetById(id.Value);
            return new OkObjectResult(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEntity(ProductViewModel viewModel)
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
                    var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Create);
                    if (result.Succeeded == false)
                        return StatusCode(401);

                    var announcementViewModel = new AnnouncementViewModel()
                    {
                        Content = $"Thêm mới sản phẩm có tên {viewModel.Name}",
                        DateCreated = DateTime.Now,
                        Status = Status.Active,
                        Title = "Thêm mới",
                        UserId = User.GetUserId(),
                        FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                        Id = Guid.NewGuid().ToString()
                    };
                    await _announcementService.AddAsync(announcementViewModel);
                    await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                    _productService.Add(viewModel);
                }
                else
                {
                    var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Update);
                    if (result.Succeeded == false)
                        return StatusCode(401);

                    var announcementViewModel = new AnnouncementViewModel()
                    {
                        Content = $"Cập nhật sản phẩm có tên {viewModel.Name}",
                        DateCreated = DateTime.Now,
                        Status = Status.Active,
                        Title = "Thêm mới",
                        UserId = User.GetUserId(),
                        FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                        Id = Guid.NewGuid().ToString()
                    };
                    await _announcementService.AddAsync(announcementViewModel);
                    await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

                    _productService.Update(viewModel);
                    isAddNew = false;
                }

                _productService.Save();
                return new OkObjectResult(isAddNew);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (id == null)
            {
                return new BadRequestResult();
            }

            var viewModel = _productService.GetById(id.Value);
            if (viewModel == null)
            {
                return new NotFoundResult();
            }

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Xóa sản phẩm có tên {viewModel.Name}",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Xóa",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            _productService.Delete(id.Value);
            _productService.Save();
            return new OkObjectResult(id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple(string jsonId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Delete);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (string.IsNullOrEmpty(jsonId))
            {
                return new BadRequestResult();
            }

            var listId = JsonConvert.DeserializeObject<List<int>>(jsonId);
            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Xóa {listId.Count} sản phẩm",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Xóa nhiều",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            _productService.DeleteMultiple(listId);
            _productService.Save();
            return new OkObjectResult(true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveQuantities(int productId, List<ProductQuantityViewModel> quantities)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Update);
            if (result.Succeeded == false)
                return StatusCode(401);

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Cập nhật số lượng sản phẩm",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Cập nhật",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            _productService.AddQuantity(productId, quantities);
            _productService.Save();
            return new OkObjectResult(quantities);
        }

        [HttpGet]
        public async Task<IActionResult> GetQuantities(int productId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var quantities = _productService.GetQuantities(productId);
            return new OkObjectResult(quantities);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveImages(int productId, string[] images)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Update);
            if (result.Succeeded == false)
                return StatusCode(401);

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Cập nhật hình ảnh sản phẩm",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Cập nhật",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            _productService.AddImages(productId, images);
            _productService.Save();

            return new OkObjectResult(images);
        }

        [HttpGet]
        public async Task<IActionResult> GetImages(int productId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var images = _productService.GetImages(productId);
            return new OkObjectResult(images);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWholePrice(int productId, List<WholePriceViewModel> wholePrices)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Update);
            if (result.Succeeded == false)
                return StatusCode(401);

            var announcementViewModel = new AnnouncementViewModel()
            {
                Content = $"Cập nhật giá bán sỉ sản phẩm",
                DateCreated = DateTime.Now,
                Status = Status.Active,
                Title = "Cập nhật",
                UserId = User.GetUserId(),
                FullName = User.GetSpecificClaim(CommonConstants.UserClaims.FullName),
                Id = Guid.NewGuid().ToString()
            };
            await _announcementService.AddAsync(announcementViewModel);
            await _hubContext.Clients.All.ReceiveMessage(announcementViewModel);

            _productService.AddWholePrice(productId, wholePrices);
            _productService.Save();
            return new OkObjectResult(wholePrices);
        }

        [HttpGet]
        public async Task<IActionResult> GetWholePrices(int productId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Read);
            if (result.Succeeded == false)
                return StatusCode(401);

            var wholePrices = _productService.GetWholePrices(productId);
            return new OkObjectResult(wholePrices);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcel(IList<IFormFile> files, int categoryId)
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Create);
            if (result.Succeeded == false)
                return StatusCode(401);

            if (files != null && files.Count > 0)
            {
                var file = files[0];
                var filename = ContentDispositionHeaderValue
                                   .Parse(file.ContentDisposition)
                                   .FileName
                                   .Trim('"');

                string folder = _hostingEnvironment.WebRootPath + $@"\uploaded\excels";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string filePath = Path.Combine(folder, filename);

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }

                _productService.ImportExcel(filePath, categoryId);
                _productService.Save();
                return new OkObjectResult(filePath);
            }

            return new NoContentResult();
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "PRODUCT_LIST", Operations.Create);
            if (result.Succeeded == false)
                return StatusCode(401);

            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string directory = Path.Combine(sWebRootFolder, "export-files");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string sFileName = $"Product_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            string fileUrl = $"{Request.Scheme}://{Request.Host}/export-files/{sFileName}";
            FileInfo file = new FileInfo(Path.Combine(directory, sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }
            var products = await _productService.GetAllAsync();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Products");
                worksheet.Cells["A1"].LoadFromCollection(products, true, TableStyles.Light1);
                worksheet.Cells.AutoFitColumns();
                package.Save(); //Save the workbook.
            }
            return new OkObjectResult(fileUrl.ToString());
        }
        #endregion
    }
}