using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Linq;

namespace AdministrationWebApi.Services.SignalR
{

    public class NotificationSignalR : Hub
    {
        private readonly IHubContext<NotificationSignalR> _hubContext;

        public NotificationSignalR(IHubContext<NotificationSignalR> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserJoined", $"{Context.ConnectionId} has joined the room {roomName}");
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserLeft", $"{Context.ConnectionId} has left the room {roomName}");
        }
     
    }
}
