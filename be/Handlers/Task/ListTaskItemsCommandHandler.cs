using MediatR;
using TaskSystem.Commands.Task;
using TaskSystem.DTO.Task;
using TaskSystem.Services;

namespace TaskSystem.Handlers.Task;

public class ListTaskItemsCommandHandler : IRequestHandler<ListTaskItemsCommand, ListTaskItemDto>
{
    private readonly IProjectManagementService _projectManagementService;

    public ListTaskItemsCommandHandler(IProjectManagementService projectManagementService)
    {
        _projectManagementService = projectManagementService;
    }

    public async Task<ListTaskItemDto> Handle(ListTaskItemsCommand command, CancellationToken cancellationToken)
    {
        return new ListTaskItemDto
        {
            Tasks = await _projectManagementService.GetTasksAsync(command.State)
        };
    }
}