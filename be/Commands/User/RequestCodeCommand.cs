using MediatR;
using TaskSystem.DTO.User.Inputs;

namespace TaskSystem.Commands.User;

public class RequestCodeCommand : IRequest
{
    public string Email { get; }

    public RequestCodeCommand(RequestCodeInput input)
    {
        Email = input.Email;
    }
}