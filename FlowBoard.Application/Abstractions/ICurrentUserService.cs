namespace FlowBoard.Application.Abstractions;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
}