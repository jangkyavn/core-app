using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface IAnnouncementService
    {
        Task AddAsync(AnnouncementViewModel viewModel);
        Task DeleteAsync(string id);
        Task DeleteMultipleAsync(List<string> ids);
        Task<List<AnnouncementViewModel>> GetAllAsync();
        Task<PagedResult<AnnouncementViewModel>> GetAllPagingAsync(string keyword, Guid userId, int page, int pageSize);
        Task<AnnouncementViewModel> GetByIdAsync(string id);
        Task<int> GetUnreadTotalAsync(Guid userId);
        void MaskAsRead(Guid userId, string id);
        void SaveChanges();
    }
}
