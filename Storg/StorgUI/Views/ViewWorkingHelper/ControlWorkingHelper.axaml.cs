using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace StorgUI;

public partial class ControlWorkingHelper : UserControl
{
    public ControlWorkingHelper()
    {
        InitializeComponent();

        this.SizeChanged += Dynamic_Change_Size;


    }

    private void Dynamic_Change_Size(object? sender, RoutedEventArgs e)
    {
       var scoll_find = this.FindControl<ScrollViewer>("ScrollBar");
       if (scoll_find != null)
       {
           scoll_find.Height = Height - 300;
       }

    }
}