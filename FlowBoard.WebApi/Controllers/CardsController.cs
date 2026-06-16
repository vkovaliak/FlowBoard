using FlowBoard.Application.Features.Cards.Commands.CreateCard;
using FlowBoard.Application.Features.Cards.Commands.DeleteCard;
using FlowBoard.Application.Features.Cards.Commands.MoveCard;
using FlowBoard.Application.Features.Cards.Commands.UpdateCard;
using FlowBoard.Domain.Constants;
using FlowBoard.WebApi.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/cards")]
[Authorize]
public class CardsConroller : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<BoardHub> _hubContext;

    public CardsConroller(IMediator mediator, IHubContext<BoardHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateCardCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(command.BoardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, command.BoardId);

        return Ok(result.Value);
    }

    [HttpPut("{boardId:guid}/list/{listId:guid}/card/{cardId:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid boardId, 
        Guid listId, 
        Guid cardId, 
        UpdateCardCommand command)
    {
        var updatedCommand = command with { 
            BoardId = boardId, 
            ListId = listId, 
            CardId = cardId 
        };
        
        var result = await _mediator.Send(updatedCommand);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpDelete("{boardId:guid}/list/{listId:guid}/card/{cardId:guid}")]
    public async Task<IActionResult> DeleteAsync(
        Guid boardId,
        Guid listId,
        Guid cardId)
    {
        var command = new DeleteCardCommand(
            BoardId: boardId,
            CardId: cardId,
            ListId: listId);
        
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpPut("{boardId:guid}/card/{cardId:guid}/move")]
    public async Task<IActionResult> MoveAsync(
        Guid boardId, 
        Guid cardId, 
        MoveCardCommand command)
    {
        var updatedCommand = new MoveCardCommand(
            BoardId: boardId,
            CardId: cardId,
            NewListId: command.NewListId,
            NewPosition: command.NewPosition
        );

        var result = await _mediator.Send(updatedCommand);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }
}
