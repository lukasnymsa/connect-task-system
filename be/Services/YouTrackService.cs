using TaskSystem.Exceptions;
using YouTrackSharp;
using YouTrackSharp.Issues;
using Issue = YouTrackSharp.Issues.Issue;
using TaskSystem.Models;
using TaskSystem.Extensions;
using TaskSystem.Enums;
using TaskSystem.Models.YouTrackItems;

namespace TaskSystem.Services;

public class YouTrackService : IProjectManagementService
{
    private const string ServiceName = "YouTrack";
    private BearerTokenConnection? _connection;
    private readonly IIssuesService _issuesService;
    private readonly IUserService _userService;
    private string UserFieldKey { get; }
    private string[] Projects { get; }
    private string MainProjectId { get; }

    public YouTrackService(IConfiguration configuration, IUserService userService)
    {
        _userService = userService;
        var serverUrl = configuration.GetValue<string>($"Config:TaskSystem:{ServiceName}:Url");
        var token = configuration.GetValue<string>($"Config:TaskSystem:{ServiceName}:Token");
        Projects = configuration.GetSection($"Config:TaskSystem:{ServiceName}:ProjectIds").Get<string[]>();
        UserFieldKey = configuration.GetValue<string>($"Config:TaskSystem:{ServiceName}:UserField");
        MainProjectId = configuration.GetValue<string>($"Config:TaskSystem:{ServiceName}:MainProjectId");
        _connection = Connect(serverUrl, token);
        _issuesService = _connection.CreateIssuesService();
    }

    public async Task<IEnumerable<TaskItem>> GetTasksAsync(TaskStateEnum? taskState)
    {
        var filters = new Dictionary<string, string>();

        if (taskState != null)
        {
            filters.Add("State", "{" + taskState.GetStringValue() + "}");
        }
        filters.Add(UserFieldKey, (await _userService.GetLoggedUserAsync()).Email);

        var filter = "";
        foreach (var item in filters)
        {
            filter += item.Key + ": " + item.Value + " ";
        }

        var list = new List<TaskItem>();
        foreach (var project in Projects)
        {
            var issuesInProject = await _issuesService.GetIssuesInProject(project, filter);
            list = list.Concat(issuesInProject.Select(
                    issue => YouTrackTaskItem.ConvertToTaskItem(issue, GetTaskStateFromIssue(issue))
                ).ToList()).ToList();
        }

        return list;
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        var issue = new YouTrackTaskItem(task, (await _userService.GetLoggedUserAsync()).Email);
        var createdIssueId = await _issuesService.CreateIssue(MainProjectId, issue);

        var createdTaskItem = await GetTaskAsync(createdIssueId);

        return createdTaskItem;
    }

    public async Task<TaskItem> GetTaskAsync(string id)
    {
        var issue = await _issuesService.GetIssue(id);

        if (issue is null)
        {
            throw new EntityNotFoundException($"Unknown entity: {id}");
        }

        var authField = (List<string>)issue.GetField(UserFieldKey).Value;

        var projectId = GetProjectId(issue);

        if (!Projects.Contains(projectId) || !authField.Contains((await _userService.GetLoggedUserAsync()).Email))
        {
            throw new EntityNotFoundException($"Unknown entity: {id}");
        }

        return YouTrackTaskItem.ConvertToTaskItem(issue, GetTaskStateFromIssue(issue));
    }

    public async Task<TaskItem> EditTaskAsync(TaskItem task)
    {
        if (task.Id is null)
        {
            throw new EntityNotFoundException($"Unknown entity: {task.Id}");
        }

        var command = "";
        if (task.TaskState.HasValue)
        {
            command += $"State {task.TaskState.GetStringValue()}";
        }

        if (task.Comments != null && task.Comments.Any())
        {
            foreach (var comment in task.Comments)
            {
                await _issuesService.AddCommentForIssue(task.Id, comment.Content);
            }
        }

        await _issuesService.ApplyCommand(task.Id, command);

        return await GetTaskAsync(task.Id);
    }

    public async Task AddCommentAsync(string id, CommentItem comment)
    {
        var task = await GetTaskAsync(id);

        if (task is null)
        {
            throw new EntityNotFoundException($"Unknown entity: {id}");
        }
        await _issuesService.AddCommentForIssue(id, comment.Content);
    }

    private BearerTokenConnection Connect(string serverUrl, string token)
    {
        _connection = new BearerTokenConnection(serverUrl, token);

        return _connection;
    }

    private static TaskStateEnum GetTaskStateFromIssue(Issue issue)
    {
        var state = issue.GetField("State").AsString();

        return EnumExtensions.GetEnumValue<TaskStateEnum>(state);
    }

    private static string GetProjectId(Issue issue)
    {
        return (string)issue.GetField("projectShortName").Value;
    }
}