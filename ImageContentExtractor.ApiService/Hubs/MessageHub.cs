using Microsoft.AspNetCore.SignalR;

namespace ImageContentExtractor.ApiService.Hubs;
//this is for client to use
public class MessageHub : Hub
{
	public async Task SendMessage(string message)
	{
		await Clients.All.SendAsync("ReceiveMessage", message);
	}
}