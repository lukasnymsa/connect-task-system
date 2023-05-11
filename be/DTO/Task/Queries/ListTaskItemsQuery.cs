using TaskSystem.Enums;

namespace TaskSystem.DTO.Task.Queries;

public class ListTaskItemsQuery
{
    public TaskStateEnum? State { get; set; }
}