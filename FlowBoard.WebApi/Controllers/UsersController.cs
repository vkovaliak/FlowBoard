using FlowBoard.Application.Features.Users.Commands.ChangePassword;
using FlowBoard.Application.Features.Users.Commands.DeleteAvatar;
using FlowBoard.Application.Features.Users.Commands.UpdateAvatar;
using FlowBoard.Application.Features.Users.Commands.UpdateUserName;
using FlowBoard.Application.Features.Users.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateAvatarAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is missing or empty.");
        }

        using var stream = file.OpenReadStream();

        var command = new UpdateAvatarCommand(stream, file.FileName);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(new { 
            avatarUrl = result.Value });
    }

    [HttpDelete("avatar")]
    public async Task<IActionResult> DeleteAvatarAsync()
    {
        var command = new DeleteAvatarCommand();
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpPut("username")]
    public async Task<IActionResult> UpdateUserNameAsync(
        UpdateUserNameCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMeAsync()
    {
        var query = new GetCurrentUserQuery();
        var result = await _mediator.Send(query);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
           return BadRequest(result.Errors.First().Message); 
        }

        return Ok(result.Value);
    }
}