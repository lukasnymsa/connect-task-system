using System.Text.Json.Serialization;

namespace TaskSystem.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskStateEnum
{
    None,
    New,
    Open,
    InProgress,
    Processed,
    Rejected,
    Retry,
    Reopened,
    Resolved
}