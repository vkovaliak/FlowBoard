using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.DTOs.Labels;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Labels.Queries.GetBoardLabels;

public class GetBoardLabelsQueryHandler
    : IRequestHandler<GetBoardLabelsQuery, Result<List<LabelDto>>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public GetBoardLabelsQueryHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<LabelDto>>> Handle(
        GetBoardLabelsQuery query, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();

        var board = await uow.BoardRepository.GetByIdAsync(query.BoardId);
        if (board is null)
        {
            return Result.Fail(ErrorMessages.BoardNotFound);
        }

        var isMember = await uow.BoardRepository.IsMemberAsync(
            query.BoardId, currentUserId);
        if (!isMember && board.CreatedBy != currentUserId)
        {
            return Result.Fail(ErrorMessages.NoBoardAccess);
        }

        var labels = await uow.LabelRepository.GetByBoardIdAsync(query.BoardId);

        return Result.Ok(labels.ToList());
    }
}