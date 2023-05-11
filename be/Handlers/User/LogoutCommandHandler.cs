using MediatR;
using TaskSystem.Commands.User;
using TaskSystem.Services;

namespace TaskSystem.Handlers.User;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IUserService _userService;

    public LogoutCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Unit> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var user = await _userService.GetLoggedUserAsync();
        await _userService.LogoutAsync(user);

        return Unit.Value;
    }
}