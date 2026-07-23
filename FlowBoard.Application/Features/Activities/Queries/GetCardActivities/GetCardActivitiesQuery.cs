using FlowBoard.Domain.DTOs.Activities;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Activities.Queries.GetCardActivities;

public record GetCardActivitiesQuery(
    Guid BoardId,
    Guid CardId) 
    : IRequest<Result<IEnumerable<ActivityDto>>>;