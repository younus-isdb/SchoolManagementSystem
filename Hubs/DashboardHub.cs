using Microsoft.AspNetCore.SignalR;

namespace SchoolManagementSystem.Hubs
{
    public class DashboardHub : Hub
    {
        public async Task PushDashboardUpdate()
        {
            await Clients.All.SendAsync("ReceiveDashboardUpdate");
        }
    }
}
