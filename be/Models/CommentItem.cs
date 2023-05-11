using TaskSystem.Commands.Task;

namespace TaskSystem.Models;

public class CommentItem
{
    public DateTime? Created { get; }
    public string? Content { get; }

    public CommentItem(DateTime? created, string? content)
    {
        Created = created;
        Content = content;
    }

    public CommentItem(AddCommentToTaskItemCommand command)
    {
        Content = command.Content;
    }

}