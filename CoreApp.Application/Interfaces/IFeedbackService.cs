using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface IFeedbackService
    {
        void Add(FeedbackViewModel feedbackVm);

        void Update(FeedbackViewModel feedbackVm);

        Task DeleteAsync(int id);

        Task DeleteMultipleAsync(List<int> ids);

        Task<List<FeedbackViewModel>> GetAllAsync();

        Task<PagedResult<FeedbackViewModel>> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<FeedbackViewModel> GetByIdAsync(int id);

        void SaveChanges();
    }
}
