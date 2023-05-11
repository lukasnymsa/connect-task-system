using MediatR;
using TaskSystem.DTO.Task;
using TaskSystem.Enums;
using TaskSystem.DTO.Task.Queries;

namespace TaskSystem.Commands.Task;

public class ListTaskItemsCommand : IRequest<ListTaskItemDto>
{
    public TaskStateEnum? State { get; }

    public ListTaskItemsCommand(TaskStateEnum? state)
    {
        State = state;
    }
}