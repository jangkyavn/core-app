using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> AddAsync(AppUserViewModel appUserViewModel);

        Task DeleteAsync(Guid id);

        Task DeleteMultiAsync(List<Guid> ids);

        Task<List<AppUserViewModel>> GetAllAsync();

        Task<PagedResult<AppUserViewModel>> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<AppUserViewModel> GetByIdAsync(Guid id);

        Task<bool> CheckExistEmail(string email);

        Task UpdateAsync(AppUserViewModel appUserViewModel);

        Task<int> GetTotalAmount();
    }
}
