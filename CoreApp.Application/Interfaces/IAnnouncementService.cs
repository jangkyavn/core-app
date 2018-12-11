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

        void MaskAsRead(Guid userId, string id);

        Task<AnnouncementViewModel> GetByIdAsync(string id);

        void SaveChanges();
    }
}
