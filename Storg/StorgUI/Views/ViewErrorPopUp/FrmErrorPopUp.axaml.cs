using System.IO;
using System.IO.Enumeration;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StorgLibs;

namespace StorgUI;

public partial class FrmErrorPopUp : Window
{

    private bool _replaceMode;
    private LibsGlobal _libsglobal = new LibsGlobal();
    private string _fileName = "";
    private bool _export = false;

    public FrmErrorPopUp()
    {
        InitializeComponent();
    }

    public FrmErrorPopUp(string message, bool replaceMode = false, string fileName = "", bool export = false) : this()
    {

        _replaceMode = replaceMode;
        cancel.IsVisible = replaceMode;
        MessageErreur.Text = message;
        _fileName = fileName;
        _export = export;

        this.Loaded += LoadBtnDyn;

        Button? btn = this.FindControl<Button>("choiceDyn");
        if (btn != null)
        {
            btn.Click += CloseErrorOrReplace;
        }

        btn = this.FindControl<Button>("cancel");
        if (btn != null)
        {
            btn.Click += Leave;
        }
    }

    private void LoadBtnDyn(object? sender, RoutedEventArgs e)
    {
        if (!_replaceMode)
        {
            choiceDyn.Content = "Ok";
        }
        else if (_replaceMode)
        {
            cancel.IsVisible = _replaceMode;
            choiceDyn.Content = "Remplacer";
            choiceDyn.Width = 90;
            if (_export)
            {
                this.Height = 170;
            }
            else
            {
                this.Height = 150;
            }
        }
    }

    private void CloseErrorOrReplace(object? sender, RoutedEventArgs e)
    {
        if (!_replaceMode)
            this.Close();
        else if (_replaceMode)
            _ = _libsglobal.ReplaceOnExportOrDownload(_fileName, !_export);
        this.Close();

    }

    private void Leave(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

}