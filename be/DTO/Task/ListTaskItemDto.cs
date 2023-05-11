using TaskSystem.Models;

namespace TaskSystem.DTO.Task;

public class ListTaskItemDto
{
    public IEnumerable<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}