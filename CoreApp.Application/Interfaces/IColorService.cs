using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface IColorService
    {
        void Add(ColorViewModel colorViewModel);

        void Update(ColorViewModel colorViewModel);

        void Delete(int id);

        void DeleteMultiple(List<int> ids);

        Task<List<ColorViewModel>> GetAllAsync();

        Task<PagedResult<ColorViewModel>> GetAllPagingAsync(string keyword, int pageIndex, int pageSize);

        Task<ColorViewModel> GetById(int id);

        void Save();
    }
}
