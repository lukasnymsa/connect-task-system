using MediatR;
using TaskSystem.Enums;
using TaskSystem.Models;

namespace TaskSystem.Commands.Task;

public class ChangeTaskStateCommand : IRequest<TaskItem>
{
    public string Id { get; }
    public TaskStateEnum State { get; }
    public string? Comment { get; set; }

    public ChangeTaskStateCommand(string id, TaskStateEnum state, string? comment)
    {
        Id = id;
        State = state;
        Comment = comment;
    }
}