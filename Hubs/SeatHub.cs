using Microsoft.AspNetCore.SignalR;

using System.Threading.Tasks;

namespace MiniCinema.Hubs
{
    public class SeatHub : Hub
    {
        public async Task JoinShowtime(string showtimeId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, showtimeId);
        }

        public async Task LeaveShowtime(string showtimeId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, showtimeId);
        }
    }
}
