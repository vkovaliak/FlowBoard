using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Cards;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Queries.GetMyCards;

public class GetMyCardsQueryHandler
    : IRequestHandler<GetMyCardsQuery, Result<List<MyCardDto>>>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyCardsQueryHandler(
        ICardRepository cardRepository,
        ICurrentUserService currentUserService)
    {
        _cardRepository = cardRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<MyCardDto>>> Handle(
        GetMyCardsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();

        var tasks = await _cardRepository.GetMyTasksAsync(currentUserId);

        return Result.Ok(tasks.ToList());
    }
}