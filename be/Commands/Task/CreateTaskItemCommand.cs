using MediatR;
using TaskSystem.Models;
using TaskSystem.DTO.Task.Inputs;

namespace TaskSystem.Commands.Task;

public class CreateTaskItemCommand : IRequest<TaskItem>
{
    public string Name { get; }
    public string Description { get; }

    public CreateTaskItemCommand(string name, string description)
    {
        Name = name;
        Description = description;
    }
}