using FluentResults;
using MediatR;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Comments;

namespace FlowBoard.Application.Features.Comments.Queries.GetCommentsByCardId;

public class GetCommentsByCardIdQueryHandler : 
    IRequestHandler<GetCommentsByCardIdQuery, Result<IEnumerable<CommentDto>>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ICardRepository _cardRepository;

    public GetCommentsByCardIdQueryHandler(ICommentRepository commentRepository, 
        ICardRepository cardRepository)
    {
        _commentRepository = commentRepository;
        _cardRepository = cardRepository;
    }

    public async Task<Result<IEnumerable<CommentDto>>> Handle(GetCommentsByCardIdQuery request, 
        CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId);
        if (card == null)
        {
            return Result.Fail("Card not found.");
        }

        var comments = await _commentRepository.GetCommentsByCardIdAsync(request.CardId);

        return Result.Ok(comments);
    }
}