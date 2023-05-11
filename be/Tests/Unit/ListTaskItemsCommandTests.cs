using Moq;
using TaskSystem.Enums;
using TaskSystem.Models;
using TaskSystem.Services;
using TaskSystem.Commands.Task;
using TaskSystem.DTO.Task.Queries;
using TaskSystem.Handlers.Task;

namespace Tests.Unit;

public class ListTaskItemsCommandTests
{
    [Fact]
    public async Task ListTaskItems_ReturnsListTaskItemDto()
    {
        var projectManagementService = new Mock<IProjectManagementService>();

        projectManagementService.Setup(x =>
            x.GetTasksAsync(It.IsAny<TaskStateEnum>())).ReturnsAsync
            (
                new List<TaskItem>
                {
                    {
                        new
                        (
                            "1",
                            "Test name1",
                            "Description lorem ipsum.",
                            TaskStateEnum.InProgress,
                            null,
                            new List<CommentItem>()
                        )
                    },
                    {
                        new
                        (
                            "2",
                            "Test name2",
                            "Description lorem ipsum.",
                            TaskStateEnum.Processed,
                            null,
                            new List<CommentItem>()
                        )
                    },
                    {
                        new
                        (
                            "3",
                            "Test name3",
                            "Description lorem ipsum.",
                            TaskStateEnum.New,
                            null,
                            new List<CommentItem>()
                        )
                    }
                }
            );

        var listTaskItemsCommandHandler = new ListTaskItemsCommandHandler(projectManagementService.Object);

        var command = new ListTaskItemsCommand(
            state: TaskStateEnum.New
        );

        var tasks = await listTaskItemsCommandHandler.Handle(command, new CancellationToken());

        Assert.NotNull(tasks.Tasks);
        Assert.Equal(3, tasks.Tasks.Count());

        projectManagementService.Verify(x =>
            x.GetTasksAsync(TaskStateEnum.New), Times.Once);
        projectManagementService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ListTaskItems_WhenNoTaskItemFounds_ReturnsEmptyList()
    {
        var projectManagementService = new Mock<IProjectManagementService>();

        projectManagementService.Setup(x =>
            x.GetTasksAsync(It.IsAny<TaskStateEnum>())).ReturnsAsync(new List<TaskItem>());

        var listTaskItemsCommandHandler = new ListTaskItemsCommandHandler(projectManagementService.Object);

        var command = new ListTaskItemsCommand(
            state: TaskStateEnum.Open
        );

        var tasks = await listTaskItemsCommandHandler.Handle(command, new CancellationToken());

        Assert.NotNull(tasks.Tasks);
        Assert.Empty(tasks.Tasks);

        projectManagementService.Verify(x =>
            x.GetTasksAsync(TaskStateEnum.Open), Times.Once);
        projectManagementService.VerifyNoOtherCalls();
    }
}