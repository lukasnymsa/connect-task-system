using Moq;
using TaskSystem.Models;
using TaskSystem.Services;
using TaskSystem.Commands.Task;
using TaskSystem.Handlers.Task;

namespace Tests.Unit;

public class AddCommentToTaskItemCommandTests
{
    [Fact]
    public async Task AddCommentToTaskItem_ReturnsVoid()
    {
        var projectManagementService = new Mock<IProjectManagementService>();

        projectManagementService.Setup(x =>
            x.AddCommentAsync(It.IsAny<string>(), It.IsAny<CommentItem>()));

        var addCommentToTaskItemCommandHandler = new AddCommentToTaskItemCommandHandler(projectManagementService.Object);

        var command = new AddCommentToTaskItemCommand(id: "2", content: "Lorem ipsum.");

        await addCommentToTaskItemCommandHandler.Handle(command, new CancellationToken());

        projectManagementService.Verify(x =>
            x.AddCommentAsync(command.Id, It.Is<CommentItem>(item => item.Content == command.Content)), Times.Once);
        projectManagementService.VerifyNoOtherCalls();
    }
}