using FlowBoard.Application.Abstractions;
using FlowBoard.Application.Features.Auth.Commands.ExternalLogin;
using FlowBoard.Application.Features.Auth.Commands.Login;
using FlowBoard.Application.Features.Auth.Commands.Logout;
using FlowBoard.Application.Features.Auth.Commands.RefreshToken;
using FlowBoard.Application.Features.Auth.Commands.Register;
using FlowBoard.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authentication;
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

    [HttpGet("external/microsoft")]
    public IActionResult ExternalMicrosoft([FromQuery] string returnUrl = "/")
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(ExternalCallback), new { returnUrl })
        };
        return Challenge(props, ExternalAuthConstants.MicrosoftScheme);
    }

    [HttpGet("external/callback")]
    public async Task<IActionResult> ExternalCallback()
    {
        var result = await HttpContext.AuthenticateAsync(
            ExternalAuthConstants.ExternalCookieScheme);

        await HttpContext.SignOutAsync(
            ExternalAuthConstants.ExternalCookieScheme);

        if (!result.Succeeded || result.Principal is null)
        {
            return Redirect(_externalAuthService.BuildErrorUrl("external"));
        }

        var userInfo = _externalAuthService.ExtractUserInfo(result.Principal);

        if (string.IsNullOrEmpty(userInfo.Email))
        {
            return Redirect(_externalAuthService.BuildErrorUrl("noemail"));
        }

        var command = new ExternalLoginCommand(
            ExternalAuthConstants.MicrosoftProvider,
            userInfo.ExternalId, userInfo.Email, userInfo.Name);

        var tokenResult = await _mediator.Send(command);

        if (tokenResult.IsFailed)
        {
            return Redirect(_externalAuthService.BuildErrorUrl("failed"));
        }

        return Redirect(_externalAuthService.BuildCallbackUrl(tokenResult.Value));
    }
}