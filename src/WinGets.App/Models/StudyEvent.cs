using System.Text.Json.Serialization;

namespace WinGets.App.Models;

public sealed class StudyEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string Title { get; set; } = string.Empty;

    public string? ModuleId { get; set; }

    public DateTimeOffset Date { get; set; } = DateTimeOffset.Now.AddDays(1);

    public string EventType { get; set; } = "Deadline";

    [JsonIgnore]
    public bool IsUrgent
    {
        get
        {
            var remaining = Date.Date - DateTimeOffset.Now.Date;
            return remaining.TotalDays >= 0 && remaining.TotalDays <= 3;
        }
    }

    [JsonIgnore]
    public string DateLabel => Date.ToString("ddd, MMM d");
}
