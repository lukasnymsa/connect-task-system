using TaskSystem.Enums;
using TaskSystem.Extensions;
using TaskSystem.Models;
using YouTrackSharp.Issues;

namespace TaskSystem.Models.YouTrackItems;

public class YouTrackTaskItem : Issue
{
    public YouTrackTaskItem(TaskItem taskItem, string user)
    {
        Summary = taskItem.Name;
        Description = taskItem.Description;
        SetField("State", taskItem.TaskState.GetStringValue());
        SetField("API-User-email", user);
        Comments = new List<Comment>();
        taskItem.Comments?.ForEach(comment => Comments.Add(
            new Comment
            {
                Text = comment.Content
            })
        );
    }

    public static TaskItem ConvertToTaskItem(Issue issue, TaskStateEnum? taskState)
    {
        var comments = issue.Comments.Select(comment =>
            new CommentItem(comment.Created?.ToLocalTime(), comment.Text)
        ).ToList();

        var timestamp = (long)issue.GetField("created").Value / 1000; // convert to milliseconds
        var created = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().DateTime;

        return new TaskItem(
            issue.Id,
            issue.Summary,
            issue.Description,
            taskState,
            created,
            comments
        );
    }
}