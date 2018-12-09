using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface IBlogService
    {
        void Add(BlogViewModel blogViewModel);

        void Update(BlogViewModel blogViewModel);

        Task DeleteAsync(int id);

        Task DeleteMultipleAsync(List<int> ids);

        Task<List<BlogViewModel>> GetAllAsync();

        Task<PagedResult<BlogViewModel>> GetAllPagingAsync(string keyword, int pageSize, int page);

        List<BlogViewModel> GetLastest(int top);

        List<BlogViewModel> GetHotProduct(int top);

        List<BlogViewModel> GetListPaging(int page, int pageSize, string sort, out int totalRow);

        List<BlogViewModel> Search(string keyword, int page, int pageSize, string sort, out int totalRow);

        List<BlogViewModel> GetList(string keyword);

        Task<List<BlogViewModel>> GetReatedBlogsAsync(int id, int top);

        List<string> GetListByName(string name);

        Task<BlogViewModel> GetByIdAsync(int id);

        void Save();

        Task<List<TagViewModel>> GetTagsByBlogIdAsync(int id);

        TagViewModel GetTag(string tagId);

        void IncreaseView(int id);

        List<BlogViewModel> GetListByTag(string tagId, int page, int pagesize, out int totalRow);

        List<TagViewModel> GetListTag(string searchText);

        PagedResult<BlogViewModel> GetBlogsPagingByTag(string tagId, int page, int pageSize);
    }
}
