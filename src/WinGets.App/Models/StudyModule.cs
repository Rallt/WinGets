using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace WinGets.App.Models;

public sealed class StudyModule
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string Name { get; set; } = string.Empty;

    public string AccentColor { get; set; } = "#8BB8FF";

    public double Progress { get; set; }

    public ObservableCollection<StudyTask> Tasks { get; set; } = [];

    [JsonIgnore]
    public double EffectiveProgress => Tasks.Count == 0
        ? Math.Clamp(Progress, 0, 100)
        : Math.Round(Tasks.Count(task => task.IsDone) * 100d / Tasks.Count);

    [JsonIgnore]
    public int CompletedTaskCount => Tasks.Count(task => task.IsDone);

    [JsonIgnore]
    public string ChecklistSummary => $"{CompletedTaskCount}/{Tasks.Count} tasks";

    [JsonIgnore]
    public int OpenTaskCount => Tasks.Count(task => !task.IsDone);

    [JsonIgnore]
    public StudyTask? NextTask => Tasks
        .Where(task => !task.IsDone)
        .OrderByDescending(task => task.Priority)
        .ThenBy(task => task.DueDate ?? DateTimeOffset.MaxValue)
        .FirstOrDefault();

    public void RefreshProgressFromTasks()
    {
        if (Tasks.Count > 0)
        {
            Progress = EffectiveProgress;
        }
    }
}
