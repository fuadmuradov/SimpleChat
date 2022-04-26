using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SimpleChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<AppUser> userManager;

        public ChatHub(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task SendMessage(string receiverUserId, string message)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = userManager.FindByNameAsync(Context.User.Identity.Name).Result;

                if (string.IsNullOrEmpty(receiverUserId))
                {
                    await Clients.All.SendAsync("ReceiveMessage", user.Fullname, message, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                }
                else
                {
                    AppUser recevierUser = userManager.FindByIdAsync(receiverUserId).Result;
                    if(recevierUser != null)
                    await Clients.Client(recevierUser.ConnectionID).SendAsync("ReceiveMessage", user.Fullname, message, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));

                }


            }

          
        }

        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {

                AppUser user = userManager.FindByNameAsync(Context.User.Identity.Name).Result;
                user.ConnectionID = Context.ConnectionId;
                var result = userManager.UpdateAsync(user).Result;
                Clients.All.SendAsync("UserConnected", user.Id);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {

                AppUser user = userManager.FindByNameAsync(Context.User.Identity.Name).Result;
                user.ConnectionID = null;
                var result = userManager.UpdateAsync(user).Result;
                Clients.All.SendAsync("UserDisConnected", user.Id);
            }
            return base.OnDisconnectedAsync(exception);
        }

    }
}
