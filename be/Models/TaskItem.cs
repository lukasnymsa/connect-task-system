using TaskSystem.Commands.Task;
using TaskSystem.Enums;

namespace TaskSystem.Models;

public class TaskItem
{
    public string? Id { get; }
    public string? Name { get; }
    public string? Description { get; }
    public TaskStateEnum? TaskState { get; set; }
    public DateTime? Created { get; }
    public List<CommentItem>? Comments { get; }

    public TaskItem(string id, string name, string description, TaskStateEnum? taskState, DateTime? created, List<CommentItem> comments)
    {
        Id = id;
        Name = name;
        Description = description;
        TaskState = taskState;
        Created = created;
        Comments = comments;
    }

    public TaskItem(CreateTaskItemCommand command)
    {
        Id = null;
        Name = command.Name;
        Description = command.Description;
        TaskState = TaskStateEnum.New;
    }

    public TaskItem(ChangeTaskStateCommand command)
    {
        Id = command.Id;
        TaskState = command.State;
        Comments = new List<CommentItem>();
        if (command.Comment is not null)
        {
            Comments.Add(new CommentItem(null, command.Comment));
        }
    }
}