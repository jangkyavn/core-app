using CoreApp.Application.ViewModels;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CoreApp.Web.SignalR
{
    public class CoreHub : Hub<ICoreHub>
    {
        public async Task SendMessage(AnnouncementViewModel message)
        {
            await Clients.All.ReceiveMessage(message);
        }
    }
}
