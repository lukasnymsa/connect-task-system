using MediatR;
using TaskSystem.DTO.User;
using TaskSystem.DTO.User.Inputs;

namespace TaskSystem.Commands.User;

public class LoginCommand : IRequest<LoginDto>
{
    public string Email { get; }
    public string Code { get; }

    public LoginCommand(LoginInput input)
    {
        Email = input.Email;
        Code = input.Code;
    }
}