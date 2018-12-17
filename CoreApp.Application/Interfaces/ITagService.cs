using CoreApp.Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface ITagService
    {
        Task<TagViewModel> GetByIdAsync(string id);
        Task<List<PopularTagViewModel>> GetPopularTagsAsync(int top);
    }
}
