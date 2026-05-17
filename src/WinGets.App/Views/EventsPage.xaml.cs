using Microsoft.UI.Xaml.Controls;

namespace WinGets.App.Views;

public sealed partial class EventsPage : Page
{
    public EventsPage()
    {
        InitializeComponent();
        DataContext = App.Current.ViewModel;
    }
}
