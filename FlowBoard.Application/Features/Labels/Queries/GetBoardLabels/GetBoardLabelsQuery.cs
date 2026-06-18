using FlowBoard.Domain.DTOs.Labels;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Labels.Queries.GetBoardLabels;

public record GetBoardLabelsQuery(
    Guid BoardId)
    : IRequest<Result<List<LabelDto>>>;