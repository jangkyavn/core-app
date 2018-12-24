﻿using CoreApp.Application.Interfaces;
using CoreApp.Utilities.Constants;
using CoreApp.Web.Extensions;
using CoreApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Web.Controllers
{
    public class CompareController : Controller
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;

        public CompareController(IProductService productService,
            IReviewService  reviewService)
        {
            _productService = productService;
            _reviewService = reviewService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region Ajax Request
        public IActionResult GetAll()
        {
            var session = HttpContext.Session.Get<List<CompareProductViewModel>>(CommonConstants.CompareSession);
            if (session == null)
                session = new List<CompareProductViewModel>();
            return new OkObjectResult(session);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCompare(int productId)
        {
            var product = _productService.GetById(productId);
       
            var session = HttpContext.Session.Get<List<CompareProductViewModel>>(CommonConstants.CompareSession);
            if (session != null)
            {
                if (!session.Any(x => x.Product.Id == productId))
                {
                    session.Add(new CompareProductViewModel() {
                        Product = product,
                        RatingAverage = await _reviewService.GetRatingAverageAsync(productId),
                        RatingTotal = await _reviewService.GetRatingTotalAsync(productId)
                    });
                }

                HttpContext.Session.Set(CommonConstants.CompareSession, session);
            }
            else
            {
                //Add new cart
                var compares = new List<CompareProductViewModel>();
                compares.Add(new CompareProductViewModel()
                {
                    Product = product,
                    RatingAverage = await _reviewService.GetRatingAverageAsync(productId),
                    RatingTotal = await _reviewService.GetRatingTotalAsync(productId)
                });
                HttpContext.Session.Set(CommonConstants.CompareSession, compares);
            }

            return new OkObjectResult(productId);
        }

        [HttpPost]
        public IActionResult RemoveFromCompare(int productId)
        {
            var session = HttpContext.Session.Get<List<CompareProductViewModel>>(CommonConstants.CompareSession);
            if (session != null)
            {
                bool hasChanged = false;
                foreach (var item in session)
                {
                    if (item.Product.Id == productId)
                    {
                        session.Remove(item);
                        hasChanged = true;
                        break;
                    }
                }
                if (hasChanged)
                {
                    HttpContext.Session.Set(CommonConstants.CompareSession, session);
                }
                return new OkObjectResult(productId);
            }
            return new EmptyResult();
        }
        #endregion
    }
}