using Moq;
using TaskSystem.Services;
using TaskSystem.Commands.User;
using TaskSystem.Handlers.User;
using TaskSystem.Models;

namespace Tests.Unit;

public class LogoutCommandTests
{
    private const string EmailAddress = "tester@test.mail";

    [Fact]
    public async Task Logout_ReturnsNoContent()
    {
        var userService = new Mock<IUserService>();

        var user = new User(EmailAddress, "a5sdf453d6sd56gf53s6d8sd56f");

        userService.Setup(x => x.GetLoggedUserAsync())
            .ReturnsAsync(user);
        userService.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()));

        var logoutCommandHandler = new LogoutCommandHandler(userService.Object);

        var command = new LogoutCommand();

        await logoutCommandHandler.Handle(command, new CancellationToken());

        userService.Verify(x => x.GetLoggedUserAsync(), Times.Once);
        userService.Verify(x => x.LogoutAsync(user), Times.Once);
        userService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Login_WhenLoggedUserNotFound_ThrowsUnauthorizedAccessException()
    {
        var userService = new Mock<IUserService>();

        var user = new User(EmailAddress, "a5sdf453d6sd56gf53s6d8sd56f");

        userService.Setup(x => x.GetLoggedUserAsync()).ThrowsAsync(new UnauthorizedAccessException());
        userService.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()));

        var logoutCommandHandler = new LogoutCommandHandler(userService.Object);

        var command = new LogoutCommand();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            logoutCommandHandler.Handle(command, new CancellationToken()));

        userService.Verify(x => x.GetLoggedUserAsync(), Times.Once);
        userService.Verify(x => x.LogoutAsync(user), Times.Never);
        userService.VerifyNoOtherCalls();
    }
}