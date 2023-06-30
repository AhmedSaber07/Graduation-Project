using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Models;

namespace Test.Services
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("DeleteRequest", message);
            await Clients.All.SendAsync("SendNewRequest", message);
            await Clients.All.SendAsync("SendNewReport", message);
        }
    }

}
