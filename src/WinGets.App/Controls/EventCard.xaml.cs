using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinGets.App.Models;

namespace WinGets.App.Controls;

public sealed partial class EventCard : UserControl
{
    public static readonly DependencyProperty EventDataProperty = DependencyProperty.Register(
        nameof(EventData), typeof(StudyEvent), typeof(EventCard), new PropertyMetadata(null, OnEventChanged));

    public EventCard()
    {
        InitializeComponent();
    }

    public StudyEvent EventData
    {
        get => (StudyEvent)GetValue(EventDataProperty);
        set => SetValue(EventDataProperty, value);
    }

    public string ModuleName => App.Current.ViewModel.GetModuleName(EventData?.ModuleId);

    public Visibility UrgencyVisibility => EventData?.IsUrgent == true ? Visibility.Visible : Visibility.Collapsed;

    public Brush CardBrush => EventData?.IsUrgent == true
        ? (Brush)Application.Current.Resources["UrgentBrush"]
        : (Brush)Application.Current.Resources["CardBackgroundBrush"];

    private static void OnEventChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        ((EventCard)dependencyObject).Bindings.Update();
    }
}
