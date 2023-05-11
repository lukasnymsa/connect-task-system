using Moq;
using TaskSystem.Enums;
using TaskSystem.Models;
using TaskSystem.Services;
using TaskSystem.Commands.Task;
using TaskSystem.Handlers.Task;

namespace Tests.Unit;

public class GetTaskItemCommandTests
{
    [Fact]
    public async Task GetTaskItem_ReturnsTaskItem()
    {
        var projectManagementService = new Mock<IProjectManagementService>();

        projectManagementService.Setup(x =>
            x.GetTaskAsync(It.IsAny<string>())).ReturnsAsync
            (
                new TaskItem
                (
                    "1",
                    "Test name",
                    "Description lorem ipsum.",
                    TaskStateEnum.Processed,
                    null,
                    new List<CommentItem>()
                )
            );

        var getTaskItemCommandHandler = new GetTaskItemCommandHandler(projectManagementService.Object);

        var command = new GetTaskItemCommand(id: "1");

        var task = await getTaskItemCommandHandler.Handle(command, new CancellationToken());

        Assert.NotNull(task);
        Assert.Equal(command.Id, task.Id);

        projectManagementService.Verify(x =>
            x.GetTaskAsync("1"), Times.Once);
        projectManagementService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetTaskItem_WhenTaskItemNotFound_ReturnsNull()
    {
        var projectManagementService = new Mock<IProjectManagementService>();

        TaskItem? returnTask = null;

        projectManagementService.Setup(x =>
            x.GetTaskAsync(It.IsAny<string>())).ReturnsAsync(returnTask);

        var getTaskItemCommandHandler = new GetTaskItemCommandHandler(projectManagementService.Object);

        var command = new GetTaskItemCommand(id: "1565");

        var task = await getTaskItemCommandHandler.Handle(command, new CancellationToken());

        Assert.Null(task);

        projectManagementService.Verify(x =>
            x.GetTaskAsync("1565"), Times.Once);
        projectManagementService.VerifyNoOtherCalls();
    }
}