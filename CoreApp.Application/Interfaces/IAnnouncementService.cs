using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface IAnnouncementService
    {
        void Add(AnnouncementViewModel viewModel);

        Task DeleteAsync(string id);

        Task DeleteMultipleAsync(List<string> ids);

        Task<List<AnnouncementViewModel>> GetAllAsync();

        Task<PagedResult<AnnouncementViewModel>> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<AnnouncementViewModel> GetByIdAsync(string id);

        void SaveChanges();
    }
}
