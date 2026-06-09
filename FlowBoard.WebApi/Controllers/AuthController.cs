using FlowBoard.Application.Features.Auth.Commands.Login;
using FlowBoard.Application.Features.Auth.Commands.Logout;
using FlowBoard.Application.Features.Auth.Commands.RefreshToken;
using FlowBoard.Application.Features.Auth.Commands.Register;
using FlowBoard.Domain.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if(result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync(RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogouthAsync(LogoutCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }
}