using FlowBoard.Domain.DTOs.Cards;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Queries.GetMyCards;

public record GetMyCardsQuery()
    : IRequest<Result<List<MyCardDto>>>;