using Microsoft.UI.Xaml;

namespace WinGets.App.Services;

public sealed class ThemeService
{
    public void ApplySystemDefault(FrameworkElement root)
    {
        // Theming is centralized here so future preset color swaps can happen without
        // scattering RequestedTheme changes through pages and controls.
        root.RequestedTheme = ElementTheme.Default;
    }
}
