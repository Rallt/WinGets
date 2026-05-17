using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Windows.Input;
using WinGets.App.Models;

namespace WinGets.App.Controls;

public sealed partial class ModuleCard : UserControl
{
    public static readonly DependencyProperty ModuleProperty = DependencyProperty.Register(
        nameof(Module), typeof(StudyModule), typeof(ModuleCard), new PropertyMetadata(null, OnModuleChanged));

    public static readonly DependencyProperty CardCommandProperty = DependencyProperty.Register(
        nameof(CardCommand), typeof(ICommand), typeof(ModuleCard), new PropertyMetadata(null));

    public ModuleCard()
    {
        InitializeComponent();
    }

    public StudyModule Module
    {
        get => (StudyModule)GetValue(ModuleProperty);
        set => SetValue(ModuleProperty, value);
    }

    public ICommand? CardCommand
    {
        get => (ICommand?)GetValue(CardCommandProperty);
        set => SetValue(CardCommandProperty, value);
    }

    public Brush AccentBrush => TryCreateBrush(Module?.AccentColor) ?? (Brush)Application.Current.Resources["PastelBlueBrush"];

    public string ProgressLabel => $"{Module?.EffectiveProgress ?? 0:0}% complete";

    private static void OnModuleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        ((ModuleCard)dependencyObject).Bindings.Update();
    }

    private static Brush? TryCreateBrush(string? color)
    {
        if (string.IsNullOrWhiteSpace(color))
        {
            return null;
        }

        try
        {
            return new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(
                255,
                Convert.ToByte(color.Substring(1, 2), 16),
                Convert.ToByte(color.Substring(3, 2), 16),
                Convert.ToByte(color.Substring(5, 2), 16)));
        }
        catch
        {
            return new SolidColorBrush(Colors.CornflowerBlue);
        }
    }
}
