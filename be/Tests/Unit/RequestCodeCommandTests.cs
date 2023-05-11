using MimeKit;
using Moq;
using TaskSystem.Commands.User;
using TaskSystem.DTO.User.Inputs;
using TaskSystem.Handlers.User;
using TaskSystem.Services;

namespace Tests.Unit;

public class RequestCodeCommandTests
{
    private const string EmailAddress = "tester@test.mail";

    [Fact]
    public async Task RequestCode_ReturnsNoContent()
    {
        var userService = new Mock<IUserService>();
        var mailService = new Mock<IMailService>();

        userService.Setup(x => x.RequestCodeAsync(It.IsAny<string>())).ReturnsAsync("JGJ54G");
        mailService.Setup(x => x.SendEmail(It.IsAny<MimeMessage>())).ReturnsAsync(true);

        var requestCodeCommandHandler = new RequestCodeCommandHandler(mailService.Object, userService.Object);

        var command = new RequestCodeCommand(new RequestCodeInput
        {
            Email = EmailAddress
        });

        await requestCodeCommandHandler.Handle(command, new CancellationToken());

        userService.Verify(x => x.RequestCodeAsync(EmailAddress), Times.Once);
        mailService.Verify(x =>
            x.SendEmail(It.Is<MimeMessage>(message =>
                message.To.Mailboxes.First().Address == EmailAddress && message.Body.ToString().Contains("JGJ54G"))), Times.Once);
        userService.VerifyNoOtherCalls();
        mailService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RequestCode_WhenMailNotSent_ThrowsException()
    {
        var userService = new Mock<IUserService>();
        var mailService = new Mock<IMailService>();

        userService.Setup(x => x.RequestCodeAsync(It.IsAny<string>())).ReturnsAsync("JGJ54G");
        mailService.Setup(x => x.SendEmail(It.IsAny<MimeMessage>())).ReturnsAsync(false);

        var requestCodeCommandHandler = new RequestCodeCommandHandler(mailService.Object, userService.Object);

        var command = new RequestCodeCommand(new RequestCodeInput
        {
            Email = EmailAddress
        });

        await Assert.ThrowsAsync<ApplicationException>(() =>
            requestCodeCommandHandler.Handle(command, new CancellationToken()));

        userService.Verify(x => x.RequestCodeAsync(EmailAddress), Times.Once);
        mailService.Verify(x =>
            x.SendEmail(It.Is<MimeMessage>(message =>
                message.To.Mailboxes.First().Address == EmailAddress && message.Body.ToString().Contains("JGJ54G"))), Times.Once);
        userService.VerifyNoOtherCalls();
        mailService.VerifyNoOtherCalls();
    }
}