using MediatR;
using TaskSystem.Commands.Task;
using TaskSystem.Models;
using TaskSystem.Services;

namespace TaskSystem.Handlers.Task;

public class GetTaskItemCommandHandler : IRequestHandler<GetTaskItemCommand, TaskItem?>
{
    private readonly IProjectManagementService _projectManagementService;

    public GetTaskItemCommandHandler(IProjectManagementService projectManagementService)
    {
        _projectManagementService = projectManagementService;
    }

    public async Task<TaskItem?> Handle(GetTaskItemCommand command, CancellationToken cancellationToken)
    {
        return await _projectManagementService.GetTaskAsync(command.Id);
    }
}