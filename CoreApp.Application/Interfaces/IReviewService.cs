using CoreApp.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface IReviewService
    {
        void Add(ReviewViewModel reviewViewModel);
        void Update(ReviewViewModel reviewViewModel);
        void Delete(int id);
        Task<List<ReviewViewModel>> GetAllAsync();
        Task<List<ReviewViewModel>> GetAllByProductIdAsync(int productId);
        Task<ReviewViewModel> GetByIdAsync(int id);
        Task<ReviewViewModel> GetByIdAsync(int productId, Guid userId);
        bool CheckExistReview(int productId, Guid userId);
        Task<int> GetRatingAverageAsync(int productId);
        Task<int> GetRatingTotalAsync(int productId);
        void Save();
    }
}
