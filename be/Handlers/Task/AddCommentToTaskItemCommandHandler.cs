using MediatR;
using TaskSystem.Models;
using TaskSystem.Commands.Task;
using TaskSystem.Services;

namespace TaskSystem.Handlers.Task;

public class AddCommentToTaskItemCommandHandler : IRequestHandler<AddCommentToTaskItemCommand>
{
    private readonly IProjectManagementService _projectManagementService;

    public AddCommentToTaskItemCommandHandler(IProjectManagementService projectManagementService)
    {
        _projectManagementService = projectManagementService;
    }

    public async Task<Unit> Handle(AddCommentToTaskItemCommand command, CancellationToken cancellationToken)
    {
        var comment = new CommentItem(command);
        await _projectManagementService.AddCommentAsync(command.Id, comment);

        return Unit.Value;
    }
}