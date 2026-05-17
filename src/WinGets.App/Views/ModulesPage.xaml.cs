using Microsoft.UI.Xaml.Controls;

namespace WinGets.App.Views;

public sealed partial class ModulesPage : Page
{
    public ModulesPage()
    {
        InitializeComponent();
        DataContext = App.Current.ViewModel;
    }
}
