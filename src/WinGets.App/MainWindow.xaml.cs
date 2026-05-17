using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinGets.App.Services;
using WinGets.App.Views;

namespace WinGets.App;

public sealed partial class MainWindow : Window
{
    private readonly ThemeService _themeService = new();

    public MainWindow()
    {
        InitializeComponent();
        ContentFrame.DataContext = App.Current.ViewModel;
        _themeService.ApplySystemDefault((FrameworkElement)Content);
        ContentFrame.Navigate(typeof(DashboardPage));
    }

    private void ShellNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is not NavigationViewItem item || item.Tag is not string tag)
        {
            return;
        }

        Type pageType = tag switch
        {
            "Modules" => typeof(ModulesPage),
            "Events" => typeof(EventsPage),
            "Zen" => typeof(ZenPage),
            "Settings" => typeof(SettingsPage),
            _ => typeof(DashboardPage)
        };

        ContentFrame.Navigate(pageType);
    }
}
