using Moq;
using TaskSystem.Enums;
using TaskSystem.Exceptions;
using TaskSystem.Models;
using TaskSystem.Services;
using TaskSystem.Commands.Task;
using TaskSystem.Handlers.Task;

namespace Tests.Unit;

public class ChangeTaskStateCommandTests
{
    [Fact]
    public async Task ChangeTaskState_ReturnsTaskItem()
    {
        var projectManagementService = new Mock<IProjectManagementService>();

        var dateTime = new DateTime();

        projectManagementService.Setup(x =>
            x.GetTaskAsync(It.IsAny<string>())).ReturnsAsync
            (
                new TaskItem
                (
                    "2",
                    "Test name",
                    "Description lorem ipsum.",
                    TaskStateEnum.Processed,
                    dateTime,
                    new List<CommentItem>()
                )
            );

        projectManagementService.Setup(x =>
            x.EditTaskAsync(It.IsAny<TaskItem>())).ReturnsAsync
        (
            new TaskItem
            (
                "2",
                "Test name",
                "Description lorem ipsum.",
                TaskStateEnum.Resolved,
                dateTime,
                new List<CommentItem>
                {
                    new (new DateTime(), "Lorem ipsum comment.")
                }
            )
        );

        var changeTaskStateCommandHandler = new ChangeTaskStateCommandHandler(projectManagementService.Object);

        var command = new ChangeTaskStateCommand
        (
            id: "2",
            state: TaskStateEnum.Resolved,
            comment: "Lorem ipsum comment."
        );

        var task = await changeTaskStateCommandHandler.Handle(command, new CancellationToken());

        Assert.Equal(command.Id, task.Id);
        Assert.Equal(command.State, task.TaskState);
        Assert.Equal(command.Comment, task.Comments?.First().Content);
        Assert.Equal("Description lorem ipsum.", task.Description);
        Assert.Equal("Test name", task.Name);
        Assert.Equal(dateTime, task.Created);

        projectManagementService.Verify(x =>
            x.GetTaskAsync(command.Id), Times.Once);
        projectManagementService.Verify(x =>
            x.EditTaskAsync(It.Is<TaskItem>(item =>
                item.TaskState == TaskStateEnum.Resolved
                && item.Comments != null
                && item.Comments.First().Content == "Lorem ipsum comment.")
            ), Times.Once);
        projectManagementService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ChangeTaskState_WhenTaskStateIsNotProcessedOrRetry_ThrowsUnprocessableDataException()
    {
        var projectManagementService = new Mock<IProjectManagementService>();

        var dateTime = new DateTime();

        projectManagementService.Setup(x =>
            x.GetTaskAsync(It.IsAny<string>())).ReturnsAsync
            (
                new TaskItem
                (
                    "2",
                    "Test name",
                    "Description lorem ipsum.",
                    TaskStateEnum.New,
                    dateTime,
                    new List<CommentItem>()
                )
            );

        var changeTaskStateCommandHandler = new ChangeTaskStateCommandHandler(projectManagementService.Object);

        var command = new ChangeTaskStateCommand
        (
            id: "2",
            state: TaskStateEnum.Resolved,
            comment: "Lorem ipsum comment."
        );

        try
        {
            await changeTaskStateCommandHandler.Handle(command, new CancellationToken());
        }
        catch (UnprocessableDataException e)
        {
            Assert.NotNull(e.Message);
        }
        catch (Exception e)
        {
            Assert.Fail("Exception not thrown!");
        }

        projectManagementService.Verify(x =>
            x.GetTaskAsync(command.Id), Times.Once);
        projectManagementService.Verify(x =>
            x.EditTaskAsync(It.IsAny<TaskItem>()), Times.Never);
        projectManagementService.VerifyNoOtherCalls();
    }
}