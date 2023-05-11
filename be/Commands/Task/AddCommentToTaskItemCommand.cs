using MediatR;

namespace TaskSystem.Commands.Task;

public class AddCommentToTaskItemCommand : IRequest
{
    public string Id { get; }
    public string Content { get; }

    public AddCommentToTaskItemCommand(string id, string content)
    {
        Id = id;
        Content = content;
    }
}