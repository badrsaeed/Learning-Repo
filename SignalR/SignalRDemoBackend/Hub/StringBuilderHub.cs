using Microsoft.AspNetCore.SignalR;


public class StringBuilderHub : Hub
{
    public async Task GetFullName(string firstName, string lastName)
    {
        await Clients.All.SendAsync("getFullName", $"{firstName} {lastName}");
    }
}
