using MediatR;
using TaskSystem.Commands.User;
using TaskSystem.DTO.User;
using TaskSystem.Services;

namespace TaskSystem.Handlers.User;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginDto>
{
    private readonly IUserService _userService;

    public LoginCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<LoginDto> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _userService.LoginAsync(command.Email, command.Code);

        return new LoginDto(user.Email, user.Token, user.ExpirationDate);
    }
}