using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public LogoutCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();
        try
        {
            var session = await uow.UserSessionRepository.GetByTokenAsync(request.RefreshToken);
            if (session is null)
            {
                return Result.Fail("Invalid refresh token.");
            }

            await uow.UserSessionRepository.DeleteAsync(session);

            uow.Commit();

            return Result.Ok(true);

        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}