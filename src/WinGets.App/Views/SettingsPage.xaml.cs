using Microsoft.UI.Xaml.Controls;
using WinGets.App.Models;
using WinGets.App.ViewModels;

namespace WinGets.App.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        DataContext = App.Current.ViewModel;
        PresetCombo.SelectedIndex = App.Current.ViewModel.Settings.ThemePreset switch
        {
            "Lavender Calm" => 1,
            "Mint Focus" => 2,
            "Ocean Mist" => 3,
            "Sunset Rose" => 4,
            "Forest Glow" => 5,
            _ => 0
        };
        LayoutCombo.SelectedIndex = (int)App.Current.ViewModel.Settings.WidgetLayoutSize;
    }

    private void PresetCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not DashboardViewModel viewModel || PresetCombo.SelectedItem is not ComboBoxItem item)
        {
            return;
        }

        viewModel.Settings.ThemePreset = item.Content?.ToString() ?? "Pastel Blue";
        viewModel.SaveCommand.Execute(null);
    }

    private void LayoutCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not DashboardViewModel viewModel)
        {
            return;
        }

        viewModel.Settings.WidgetLayoutSize = (WidgetLayoutSize)Math.Max(0, LayoutCombo.SelectedIndex);
        viewModel.SaveCommand.Execute(null);
    }
}
