namespace WinGets.App.Models;

public sealed class StudyTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string Title { get; set; } = string.Empty;

    public bool IsDone { get; set; }

    public string? Notes { get; set; }
}
