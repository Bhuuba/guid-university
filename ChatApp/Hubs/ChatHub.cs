using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Data;
using ChatApp.Models;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }        public async Task LoadMessages()
        {
            var messages = await _context.Messages
                .OrderBy(m => m.Timestamp)
                .Take(50)
                .ToListAsync();
            await Clients.Caller.SendAsync("LoadMessages", messages);
        }

        public async Task SendMessage(string user, string message)
        {
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(message))
                return;

            var newMessage = new Message
            {
                UserName = user,
                Text = message,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", user, message, newMessage.Timestamp);
        }
    }
}
