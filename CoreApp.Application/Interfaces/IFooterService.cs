using CoreApp.Application.ViewModels;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface IFooterService
    {
        void Add(FooterViewModel footerViewModel);

        void Update(FooterViewModel footerViewModel);

        void Delete(string id);

        Task<FooterViewModel> GetFooterAsync();

        Task<bool> HasDataAsync();

        void Save();
    }
}
