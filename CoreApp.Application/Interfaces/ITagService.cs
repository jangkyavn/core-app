using CoreApp.Application.ViewModels;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface ITagService
    {
        Task<TagViewModel> GetByIdAsync(string id);
    }
}
