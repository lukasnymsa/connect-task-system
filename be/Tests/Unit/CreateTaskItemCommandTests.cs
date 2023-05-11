using Moq;
using TaskSystem.Enums;
using TaskSystem.Models;
using TaskSystem.Services;
using TaskSystem.Commands.Task;
using TaskSystem.Handlers.Task;

namespace Tests.Unit;

public class CreateTaskItemCommandTests
{
    [Fact]
    public async Task CreateTaskItem_ReturnsTaskItem()
    {
        var projectManagementService = new Mock<IProjectManagementService>();

        projectManagementService.Setup(x =>
            x.CreateTaskAsync(It.IsAny<TaskItem>())).ReturnsAsync
            (
                new TaskItem
                (
                    "1",
                    "Test name",
                    "Description lorem ipsum.",
                    TaskStateEnum.New,
                    null,
                    new List<CommentItem>()
                )
            );

        var createTaskItemCommandHandler = new CreateTaskItemCommandHandler(projectManagementService.Object);

        var command = new CreateTaskItemCommand(
            name: "Test name",
            description: "Description lorem ipsum."
        );

        var task = await createTaskItemCommandHandler.Handle(command, new CancellationToken());

        Assert.Equal(command.Description, task.Description);
        Assert.Equal(command.Name, task.Name);

        projectManagementService.Verify(x =>
            x.CreateTaskAsync(It.Is<TaskItem>(taskItem =>
                taskItem.Description == command.Description &&
                taskItem.Name == command.Name)
            ), Times.Once);
        projectManagementService.VerifyNoOtherCalls();
    }
}