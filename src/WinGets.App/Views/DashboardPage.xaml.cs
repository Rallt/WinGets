using Microsoft.UI.Xaml.Controls;

namespace WinGets.App.Views;

public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        InitializeComponent();
        DataContext = App.Current.ViewModel;
    }
}
