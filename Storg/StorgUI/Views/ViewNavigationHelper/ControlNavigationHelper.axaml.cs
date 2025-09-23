using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace StorgUI;

public partial class ControlNavigationHelper : UserControl
{

    //private Control? tmp_page;

    public ControlNavigationHelper()
    {
        InitializeComponent();

        //tmp_page = MainContentNav.Content as Control;

        //this.SizeChanged += Dynamic_Change_Size;


        //var btnFonc = this.FindControl<Button>("Fonc");
        //if (btnFonc != null)
        //{
        //    btnFonc.Click += GoToFonc;
        //    btnFonc.GotFocus += (sender, e) =>
        //    {

        //        btnFonc.Background = new SolidColorBrush(Color.Parse("#bca7a7"));
        //    };
        //    btnFonc.LostFocus += (sender, e) =>
        //    {
        //        btnFonc.Background = new SolidColorBrush(Color.Parse("#d6bebe"));
        //    };

        //}
        //var btnNav = this.FindControl<Button>("Nav");
        //if (btnNav != null)
        //{
        //    this.Loaded += (sender, e) => btnNav.Focus();

        //    btnNav.GotFocus += (sender, e) =>
        //    {
        //        btnNav.Background = new SolidColorBrush(Color.Parse("#bca7a7"));
        //    };
        //    btnNav.LostFocus += (sender, e) =>
        //    {
        //        btnNav.Background = new SolidColorBrush(Color.Parse("#d6bebe"));
        //    };
        //    btnNav.Click += SetAideNav;
        //}




    }


    //private void Dynamic_Change_Size(object? sender, RoutedEventArgs e)
    //{
    //    var scoll_find = this.FindControl<ScrollViewer>("ScrollBar");
    //    if (scoll_find != null)
    //    {
    //        scoll_find.Height = Height - 300;
    //    }

    //}

    //private void GoToFonc(object? sender, RoutedEventArgs e)
    //{

    //    MainContentNav.Content = new AideFonc();


    //}

    //private void SetAideNav(object? sender, RoutedEventArgs e)
    //{
    //    MainContentNav.Content = tmp_page;


    //}

}