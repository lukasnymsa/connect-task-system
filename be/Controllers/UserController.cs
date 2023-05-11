using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskSystem.Commands.User;
using TaskSystem.DTO.User;
using TaskSystem.DTO.User.Inputs;

namespace TaskSystem.Controllers;

[Route("api/v1/users")]
[ApiController]
public class UserController : ControllerBase
{

    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(template: "request-code")]
    public async Task<ActionResult> RequestCode([FromBody] RequestCodeInput input)
    {
        var command = new RequestCodeCommand(input);

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost(template: "login")]
    public async Task<ActionResult<LoginDto>> Login([FromBody] LoginInput input)
    {
        var command = new LoginCommand(input);

        return Ok(await _mediator.Send(command));
    }

    [HttpPost(template: "logout")]
    public async Task<ActionResult<LoginDto>> Logout()
    {
        var command = new LogoutCommand();

        await _mediator.Send(command);

        return NoContent();
    }
}