using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface IRoleService
    {
        Task<bool> AddAsync(AppRoleViewModel appRoleViewModel);
        Task DeleteAsync(Guid id);
        Task<List<AppRoleViewModel>> GetAllAsync();
        Task<PagedResult<AppRoleViewModel>> GetAllPagingAsync(string keyword, int page, int pageSize);
        Task<AppRoleViewModel> GetById(Guid id);
        Task UpdateAsync(AppRoleViewModel appRoleViewModel);
        Task<List<PermissionViewModel>> GetListFunctionWithRoleAsync(Guid roleId);
        void SavePermission(List<PermissionViewModel> permissions, Guid roleId);
        Task<bool> CheckPermission(string functionId, string action, string[] roles);
    }
}
