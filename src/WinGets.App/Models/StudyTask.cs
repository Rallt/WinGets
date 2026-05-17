using System.Text.Json.Serialization;

namespace WinGets.App.Models;

public sealed class StudyTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string Title { get; set; } = string.Empty;

    public bool IsDone { get; set; }

    public string? Notes { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public int Priority { get; set; } = 1;

    public int EstimatedMinutes { get; set; } = 25;

    [JsonIgnore]
    public bool IsDueSoon => DueDate is not null && DueDate.Value.Date <= DateTimeOffset.Now.Date.AddDays(3);

    [JsonIgnore]
    public string Metadata => DueDate is null
        ? $"Priority {Priority} · {EstimatedMinutes} min"
        : $"Due {DueDate.Value:MMM d} · Priority {Priority} · {EstimatedMinutes} min";
}
