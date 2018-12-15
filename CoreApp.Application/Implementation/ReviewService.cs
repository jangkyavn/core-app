using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;
using CoreApp.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Application.Implementation
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(ReviewViewModel reviewViewModel)
        {
            var model = _mapper.Map<ReviewViewModel, Review>(reviewViewModel);
            _reviewRepository.Add(model);
        }

        public bool CheckExistReview(int productId, Guid userId)
        {
            return _reviewRepository.FindSingle(x => x.ProductId == productId && x.UserId == userId) != null;
        }

        public void Delete(int id)
        {
            _reviewRepository.Remove(id);
        }

        public async Task<List<ReviewViewModel>> GetAllAsync()
        {
            return await _reviewRepository.FindAll()
                .OrderByDescending(x => x.DateCreated)
                .ProjectTo<ReviewViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<ReviewViewModel>> GetAllByProductIdAsync(int productId)
        {
            return await _reviewRepository.FindAll(x => x.ProductId == productId, x => x.AppUser)
                .OrderByDescending(x => x.DateCreated)
                .ProjectTo<ReviewViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ReviewViewModel> GetByIdAsync(int id)
        {
            var model = await _reviewRepository.FindByIdAsync(id);
            return _mapper.Map<Review, ReviewViewModel>(model);
        }

        public async Task<ReviewViewModel> GetByIdAsync(int productId, Guid userId)
        {
            var model = await _reviewRepository.FindSingleAsync(x => x.ProductId == productId && x.UserId == userId);
            return _mapper.Map<Review, ReviewViewModel>(model);
        }

        public async Task<int> GetRatingAverageAsync(int productId)
        {
            var query = _reviewRepository.FindAll(x => x.ProductId == productId);

            if (query.Count() > 0)
            {
                return (int)Math.Round(await query.AverageAsync(x => x.Rating));
            }

            return 0;
        }

        public async Task<int> GetRatingTotalAsync(int productId)
        {
            return await _reviewRepository.FindAll(x => x.ProductId == productId).CountAsync();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(ReviewViewModel reviewViewModel)
        {
            var model = _mapper.Map<ReviewViewModel, Review>(reviewViewModel);
            _reviewRepository.Update(model);
        }
    }
}
