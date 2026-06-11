using Microsoft.AspNetCore.SignalR;

namespace FlowBoard.WebApi.Hubs;

public class CommentHub : Hub
{
    public async Task JoinCardComments(Guid cardId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId, cardId.ToString());
    }

    public async Task LeaveCardComments(Guid cardId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId, cardId.ToString());
    }
}