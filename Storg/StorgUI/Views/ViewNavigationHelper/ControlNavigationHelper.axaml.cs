using System.Collections;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace StorgUI;

public partial class ControlNavigationHelper : UserControl
{


    public ControlNavigationHelper()
    {
        InitializeComponent();
        this.SizeChanged += Dynamic_Change_Size;

        Button btnFonc = this.FindControl<Button>("Fonc")!;
        if (btnFonc != null)
        {
            btnFonc.GotFocus += (sender, e) => btnFonc.Background = GetThemeBrush("HoverBackground");
            btnFonc.LostFocus += (sender, e) => btnFonc.Background = GetThemeBrush("NavBackground");
            btnFonc.Click += GoToFonc;
        }

        Button btnNav = this.FindControl<Button>("Nav")!;
        if (btnNav != null)
        {
            this.Loaded += (sender, e) => btnNav.Focus();
            btnNav.GotFocus += (sender, e) => btnNav.Background = GetThemeBrush("HoverBackground");
            btnNav.LostFocus += (sender, e) => btnNav.Background = GetThemeBrush("NavBackground");
            btnNav.Click += SetAideNav;
        }
    }

    private SolidColorBrush GetThemeBrush(string key)
    {
        var themeVariant = Application.Current!.ActualThemeVariant;
        if (Application.Current.TryGetResource(key, themeVariant, out var resource)
            && resource is SolidColorBrush brush)
        {
            return brush;
        }
        // Fallback si la ressource n'est pas trouvée
        return new SolidColorBrush(Colors.Transparent);
    }

    private void Dynamic_Change_Size(object? sender, RoutedEventArgs e)
    {
        ScrollViewer? scoll_find = this.FindControl<ScrollViewer>("ScrollBar");
        if (scoll_find != null)
        {
            scoll_find.Height = Height - 300;
        }
    }

    private void GoToFonc(object? sender, RoutedEventArgs e)
    {
        MainContentNav.Content = new ControlWorkingHelper().Content;
    }

    private void SetAideNav(object? sender, RoutedEventArgs e)
    {
        MainContentNav.Content = new ControlNavigationHelper();
    }

}