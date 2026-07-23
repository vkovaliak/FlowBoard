using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Activities;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Activities.Queries.GetCardActivities;

public class GetCardActivitiesQueryHandler
    : IRequestHandler<GetCardActivitiesQuery, Result<IEnumerable<ActivityDto>>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetCardActivitiesQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<ActivityDto>>> Handle(
        GetCardActivitiesQuery command, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var activities = await uow.ActivityRepository
            .GetByCardIdAsync(command.CardId);

        return Result.Ok(activities);
    }
}