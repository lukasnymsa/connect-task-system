using System.Security.Authentication;
using Moq;
using TaskSystem.Services;
using TaskSystem.Commands.User;
using TaskSystem.DTO.User.Inputs;
using TaskSystem.Handlers.User;
using TaskSystem.Models;

namespace Tests.Unit;

public class LoginCommandTests
{
    private const string EmailAddress = "tester@test.mail";

    [Fact]
    public async Task Login_ReturnsLoginDto()
    {
        var userService = new Mock<IUserService>();

        userService.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new User(EmailAddress, "h5uh34jad75jk4ad76fs432sa7d"));

        var loginCommandHandler = new LoginCommandHandler(userService.Object);

        var command = new LoginCommand(new LoginInput
        {
            Code = "JGJ54G",
            Email = EmailAddress
        });

        var loginDto = await loginCommandHandler.Handle(command, new CancellationToken());

        Assert.Equal(EmailAddress, loginDto.Email);
        Assert.Equal("h5uh34jad75jk4ad76fs432sa7d", loginDto.Token);

        userService.Verify(x => x.LoginAsync(EmailAddress, "JGJ54G"), Times.Once);
        userService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Login_WhenRequestCodeNotFound_ThrowsAuthenticationException()
    {
        var userService = new Mock<IUserService>();

        userService.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new AuthenticationException());

        var loginCommandHandler = new LoginCommandHandler(userService.Object);

        var command = new LoginCommand(new LoginInput
        {
            Code = "HG56SS",
            Email = EmailAddress
        });

        await Assert.ThrowsAsync<AuthenticationException>(() =>
            loginCommandHandler.Handle(command, new CancellationToken()));

        userService.Verify(x => x.LoginAsync(EmailAddress, "HG56SS"), Times.Once);
        userService.VerifyNoOtherCalls();
    }
}