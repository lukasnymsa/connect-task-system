using MediatR;
using MimeKit;
using TaskSystem.Commands.User;
using TaskSystem.Services;
using TaskSystem.Config;

namespace TaskSystem.Handlers.User;

public class RequestCodeCommandHandler : IRequestHandler<RequestCodeCommand>
{
    private readonly IMailService _mailService;
    private readonly IUserService _userService;

    public RequestCodeCommandHandler(IMailService mailService, IUserService userService)
    {
        _mailService = mailService;
        _userService = userService;
    }

    public async Task<Unit> Handle(RequestCodeCommand command, CancellationToken cancellationToken)
    {
        var code = await _userService.RequestCodeAsync(command.Email);

        var message = new MimeMessage();
        try
        {
            message.To.Add(new MailboxAddress(null, command.Email));
        }
        catch (Exception)
        {
            throw new InvalidDataException("Invalid email.");
        }
        message.Subject = "Code for login";
        message.Body = new TextPart("plain")
        {
            Text = "Code: " + code
        };

        bool emailSent;
        try
        {
            emailSent = await _mailService.SendEmail(message);
        }
        catch (Exception)
        {
            throw new InvalidDataException("Could not send an email to provided mail address.");
        }

        if (!emailSent)
        {
            throw new ApplicationException("Could not send email.");
        }

        return Unit.Value;
    }
}