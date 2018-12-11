using System.Threading.Tasks;
using CoreApp.Application.ViewModels;

namespace CoreApp.Web.SignalR
{
    public interface ICoreHub
    {
        Task ReceiveMessage(AnnouncementViewModel message);
    }
}