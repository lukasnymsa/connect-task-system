using MediatR;
using TaskSystem.Models;

namespace TaskSystem.Commands.Task;

public class GetTaskItemCommand : IRequest<TaskItem>
{
    public string Id { get; }

    public GetTaskItemCommand(string id)
    {
        Id = id;
    }
}