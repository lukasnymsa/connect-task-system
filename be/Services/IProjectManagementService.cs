using TaskSystem.Enums;
using TaskSystem.Models;

namespace TaskSystem.Services;

public interface IProjectManagementService
{
    Task<IEnumerable<TaskItem>> GetTasksAsync(TaskStateEnum? taskState);

    Task<TaskItem> CreateTaskAsync(TaskItem taskItem);

    Task<TaskItem> GetTaskAsync(string id);

    Task<TaskItem> EditTaskAsync(TaskItem task);

    Task AddCommentAsync(string id, CommentItem comment);
}