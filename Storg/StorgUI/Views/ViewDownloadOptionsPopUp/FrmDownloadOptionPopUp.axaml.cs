using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace StorgUI;

public partial class OptionPopUp : Window
{
    public OptionPopUp()
    {
        InitializeComponent();

        //var button = this.FindControl<Button>("Exporter");
        //if (button != null)
        //{
        //    button.Click += OnclickExp;
        //}

        //button = this.FindControl<Button>("Telecharger");
        //if (button != null)
        //{
        //    button.Click += OnclickDl;
        //}

        //button = this.FindControl<Button>("Supprimer");
        //if (button != null)
        //{
        //    button.Click += OnclickDel;
        //}
    }

    //private void OnclickDel(object? sender, RoutedEventArgs e)
    //{
    //    MainWindow.Delete_File();
    //}

    //private void OnclickDl(object? sender, RoutedEventArgs e)
    //{
    //    MainWindow.Download_File();
    //}

    //private void OnclickExp(object? sender, RoutedEventArgs e)
    //{
    //    MainWindow.Export_File();
    //}
}