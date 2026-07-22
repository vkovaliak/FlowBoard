using MediatR;
using Microsoft.Extensions.Logging;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Features.Activities.Events;

public class BoardActivityEventHandler : INotificationHandler<BoardActivityEvent>
{
    private readonly IActivityRepository _activityRepository;
    private readonly ILogger<BoardActivityEventHandler> _logger;

    public BoardActivityEventHandler(
        IActivityRepository activityRepository,
        ILogger<BoardActivityEventHandler> logger)
    {
        _activityRepository = activityRepository;
        _logger = logger;
    }

    public async Task Handle(BoardActivityEvent activityEvent, CancellationToken ct)
    {
        try
        {
            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                BoardId = activityEvent.BoardId,
                UserId = activityEvent.UserId,
                ActionType = activityEvent.ActionType,
                EntityType = activityEvent.EntityType,
                EntityId = activityEvent.EntityId,
                Description = activityEvent.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _activityRepository.CreateAsync(activity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to write activity log for board {BoardId}", activityEvent.BoardId);
        }
    }
}