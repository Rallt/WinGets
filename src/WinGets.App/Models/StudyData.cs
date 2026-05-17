using System.Collections.ObjectModel;

namespace WinGets.App.Models;

public sealed class StudyData
{
    public ObservableCollection<StudyModule> Modules { get; set; } = [];

    public ObservableCollection<StudyEvent> Events { get; set; } = [];

    public AppSettings Settings { get; set; } = new();
}
