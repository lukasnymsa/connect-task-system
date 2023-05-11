using MediatR;
using TaskSystem.Commands.Task;
using TaskSystem.Models;
using TaskSystem.Services;

namespace TaskSystem.Handlers.Task;

public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, TaskItem>
{
    private readonly IProjectManagementService _projectManagementService;

    public CreateTaskItemCommandHandler(IProjectManagementService projectManagementService)
    {
        _projectManagementService = projectManagementService;
    }

    public async Task<TaskItem> Handle(CreateTaskItemCommand command, CancellationToken cancellationToken)
    {
        return await _projectManagementService.CreateTaskAsync(new TaskItem(command));
    }
}