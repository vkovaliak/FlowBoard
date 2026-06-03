using FlowBoard.Application.Features.Cards.Commands.CreateCard;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/cards")]
public class CardsConroller : ControllerBase
{
    private readonly IMediator _mediator;

    public CardsConroller(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateCardCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }
}
