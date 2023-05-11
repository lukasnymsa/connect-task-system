using MediatR;
using TaskSystem.Commands.Task;
using TaskSystem.Enums;
using TaskSystem.Models;
using TaskSystem.Services;
using TaskSystem.Exceptions;

namespace TaskSystem.Handlers.Task;

public class ChangeTaskStateCommandHandler : IRequestHandler<ChangeTaskStateCommand, TaskItem>
{
    private readonly IProjectManagementService _projectManagementService;

    public ChangeTaskStateCommandHandler(IProjectManagementService projectManagementService)
    {
        _projectManagementService = projectManagementService;
    }

    public async Task<TaskItem> Handle(ChangeTaskStateCommand command, CancellationToken cancellationToken)
    {
        var task = await _projectManagementService.GetTaskAsync(command.Id);

        if (task.TaskState != TaskStateEnum.Processed && task.TaskState != TaskStateEnum.Retry)
        {
            throw new UnprocessableDataException(
                $"Could not change state to {command.State} from {task.TaskState}");
        }

        return await _projectManagementService.EditTaskAsync(new TaskItem(command));
    }
}