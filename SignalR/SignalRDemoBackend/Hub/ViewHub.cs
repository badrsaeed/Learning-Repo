using Microsoft.AspNetCore.SignalR;

public class ViewHub : Hub{
    public static int ViewCounter {get;set;} = 0;


    public async Task NotifyWatching(){
        ViewCounter++;
        await Clients.All.SendAsync("updateViewCounter",ViewCounter);
    }
}