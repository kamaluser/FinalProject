using Microsoft.AspNetCore.SignalR;

namespace Cinema.UI.Hubs
{
    public class BookingHub : Hub
    {
        public async Task NotifyAdmin(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
