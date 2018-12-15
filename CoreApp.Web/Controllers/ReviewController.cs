using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;
using CoreApp.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Web.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly SignInManager<AppUser> _signInManager;

        public ReviewController(IReviewService reviewService,
            SignInManager<AppUser> signInManager)
        {
            _reviewService = reviewService;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region Ajax Request
        [HttpGet]
        public async Task<IActionResult> GetAllByProductId(int productId)
        {
            var viewModels = await _reviewService.GetAllByProductIdAsync(productId);
            return new OkObjectResult(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var viewModel = await _reviewService.GetByIdAsync(id);
            return new OkObjectResult(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CheckExistReview(int productId)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return new OkObjectResult(new
                {
                    Status = false
                });
            }

            var result = _reviewService.CheckExistReview(productId, User.GetUserId());
            var viewModel = await _reviewService.GetByIdAsync(productId, User.GetUserId());

            return new OkObjectResult(new
            {
                Status = result,
                Data = viewModel
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveChange(ReviewViewModel reviewViewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            var isAddNew = true;
            if (reviewViewModel.Id == 0)
            {
                reviewViewModel.UserId = User.GetUserId();
                _reviewService.Add(reviewViewModel);
            }
            else
            {
                isAddNew = false;
                reviewViewModel.UserId = User.GetUserId();
                _reviewService.Update(reviewViewModel);
            }

            _reviewService.Save();

            return new OkObjectResult(isAddNew);
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            _reviewService.Delete(id.Value);
            _reviewService.Save();

            return new OkObjectResult(true);
        }
        #endregion
    }
}