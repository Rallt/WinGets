using Microsoft.UI.Xaml.Controls;

namespace WinGets.App.Views;

public sealed partial class ZenPage : Page
{
    public ZenPage()
    {
        InitializeComponent();
        DataContext = App.Current.ViewModel;
    }
}
