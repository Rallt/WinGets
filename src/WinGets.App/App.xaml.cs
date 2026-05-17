using Microsoft.UI.Xaml;
using WinGets.App.Services;
using WinGets.App.ViewModels;

namespace WinGets.App;

public partial class App : Application
{
    private Window? _window;

    public App()
    {
        InitializeComponent();
        ViewModel = new DashboardViewModel(new StudyDataService());
    }

    public static new App Current => (App)Application.Current;

    public DashboardViewModel ViewModel { get; }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        await ViewModel.InitializeAsync();
        _window = new MainWindow();
        _window.Activate();
    }
}
