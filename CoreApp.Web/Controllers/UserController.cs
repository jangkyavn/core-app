using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Web.Extensions;
using CoreApp.Web.Models.CustomerAddress;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public UserController(IUserService userService,
            IHostingEnvironment hostingEnvironment)
        {
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
        }

        #region Ajax Request
        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return new BadRequestResult();
            }

            var model = await _userService.GetByIdAsync(id);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult LoadCities()
        {
            string path = _hostingEnvironment.WebRootPath + "\\json\\city.json";
            var cities = new List<City>().Deserialize(path);

            return new OkObjectResult(cities.OrderBy(x => x.Name));
        }

        [HttpGet]
        public IActionResult LoadDistricts(int? cityId)
        {
            string path = _hostingEnvironment.WebRootPath + "\\json\\district.json";
            var districts = new List<District>().Deserialize(path);

            if (cityId.HasValue)
            {
                districts = districts.Where(x => x.Parent_Code == cityId.Value).OrderBy(x => x.Name).ToList();
            }

            return new OkObjectResult(districts);
        }

        [HttpGet]
        public IActionResult LoadWards(int? districtId)
        {
            string path = _hostingEnvironment.WebRootPath + "\\json\\ward.json";
            var wards = new List<Ward>().Deserialize(path);

            if (districtId.HasValue)
            {
                wards = wards.Where(x => x.Parent_Code == districtId).OrderBy(x => x.Name).ToList();
            }

            return new OkObjectResult(wards);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AppUserViewModel appUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var viewModel = await _userService.GetByIdAsync(User.GetUserId());
                viewModel.FullName = appUserViewModel.FullName;
                viewModel.PhoneNumber = appUserViewModel.PhoneNumber;

                await _userService.UpdateAsync(viewModel);
                return new OkObjectResult(true);
            }

            return new OkObjectResult(false);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAddress(string address)
        {
            if (ModelState.IsValid)
            {
                var viewModel = await _userService.GetByIdAsync(User.GetUserId());
                viewModel.Address = address;

                await _userService.UpdateAsync(viewModel);
                return new OkObjectResult(true);
            }

            return new OkObjectResult(false);
        }
        #endregion
    }
}