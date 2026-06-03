using FlowBoard.Application.Features.Auth.Commands.Login;
using FlowBoard.Application.Features.Auth.Commands.RefreshToken;
using FlowBoard.Application.Features.Auth.Commands.Register;
using FlowBoard.Domain.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterCommand command)
    {
        var tokens = await _mediator.Send(command);

        return Ok(tokens.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginCommand command)
    {
        var tokens = await _mediator.Send(command);
        
        return Ok(tokens.Value);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync(RefreshTokenCommand command)
    {
        var tokens = await _mediator.Send(command);
        
        return Ok(tokens.Value);
    }
}