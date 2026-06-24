using FlowBoard.Application.Abstractions;
using FlowBoard.Application.Features.Auth.Commands.ExternalLogin;
using FlowBoard.Application.Features.Auth.Commands.Login;
using FlowBoard.Application.Features.Auth.Commands.Logout;
using FlowBoard.Application.Features.Auth.Commands.RefreshToken;
using FlowBoard.Application.Features.Auth.Commands.Register;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IExternalAuthService _externalAuthService;


    public AuthController(IMediator mediator, IExternalAuthService externalAuthService)
    {
        _mediator = mediator;
        _externalAuthService = externalAuthService;
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

    [HttpPost("external/microsoft")]
    public async Task<IActionResult> ExternalMicrosoftAsync(ExternalTokenDto dto)
    {
        var principal = await _externalAuthService
            .ValidateMicrosoftTokenAsync(dto.IdToken);

        if (principal is null)
        {
            return Unauthorized("Invalid external token.");
        }

        var userInfo = _externalAuthService.ExtractUserInfo(principal);

        if (string.IsNullOrEmpty(userInfo.Email))
        {
            return BadRequest("Email not found in external token.");
        }

        var command = new ExternalLoginCommand(
            ExternalAuthConstants.MicrosoftProvider,
            userInfo.ExternalId, userInfo.Email, userInfo.Name);

        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }
}