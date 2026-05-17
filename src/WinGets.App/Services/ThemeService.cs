using Microsoft.UI.Xaml;

namespace WinGets.App.Services;

public sealed class ThemeService
{
    public static IReadOnlyList<string> ThemePresets { get; } = ["Pastel Blue", "Lavender Calm", "Mint Focus", "Ocean Mist", "Sunset Rose", "Forest Glow"];

    public string GetPresetResourceKey(string? presetName) => presetName switch
    {
        "Lavender Calm" => "PastelLavenderBrush",
        "Mint Focus" => "PastelMintBrush",
        "Ocean Mist" => "OceanMistBrush",
        "Sunset Rose" => "SunsetRoseBrush",
        "Forest Glow" => "ForestGlowBrush",
        _ => "PastelBlueBrush"
    };

    public void ApplySystemDefault(FrameworkElement root)
    {
        // Theming is centralized here so future preset color swaps can happen without
        // scattering RequestedTheme changes through pages and controls.
        root.RequestedTheme = ElementTheme.Default;
    }
}
