using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface ISizeService
    {
        void Add(SizeViewModel SizeViewModel);

        void Update(SizeViewModel SizeViewModel);

        void Delete(int id);

        void DeleteMultiple(List<int> ids);

        Task<List<SizeViewModel>> GetAllAsync();

        Task<PagedResult<SizeViewModel>> GetAllPagingAsync(string keyword, int pageIndex, int pageSize);

        Task<SizeViewModel> GetById(int id);

        void Save();
    }
}
