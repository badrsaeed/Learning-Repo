using Microsoft.AspNetCore.SignalR;

public class ViewHub : Hub
{
    private readonly IHostEnvironment _environment;

    public static int ViewCounter { get; set; } = 0;

    public ViewHub(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task NotifyWatching()
    {
        ViewCounter++;
        // ??? ???????? ???? ??????? ????? ?? ??????? ??? updateViewCounter
        await Clients.All.SendAsync("updateViewCounter", ViewCounter);
    }
    public async Task<string> GetApplicationName()
    {
        var value = $"{_environment.ApplicationName} {ViewCounter}";

        return value;
    }
}