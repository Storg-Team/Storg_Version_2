using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace StorgUI;

public partial class FrmErrorPopUp : Window
{
    public FrmErrorPopUp()
    {
        InitializeComponent();

        var btn = this.FindControl<Button>("leave");
        if (btn != null)
        {
           btn.Click += Leave;
        }
    }


    private void Leave(object? sender, RoutedEventArgs e)
    {
       this.Close();
    }

}