namespace WinGets.App.Models;

public enum WidgetLayoutSize
{
    Compact,
    Medium,
    Large
}

public sealed class AppSettings
{
    public string ThemePreset { get; set; } = "Pastel Blue";

    public WidgetLayoutSize WidgetLayoutSize { get; set; } = WidgetLayoutSize.Medium;

    public bool ShowExams { get; set; } = true;

    public bool ShowTasks { get; set; } = true;

    public bool ShowProgress { get; set; } = true;

    public bool EnableZenAnimations { get; set; } = true;
}
