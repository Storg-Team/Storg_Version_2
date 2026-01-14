using System.IO;
using System.IO.Enumeration;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StorgCommon;
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

        choiceDyn.Click += CloseErrorOrReplaceOnExportOrDownload;
        cancel.Click += Leave;
    }

    public FrmErrorPopUp(string message, string fileName, bool import) : this()
    {
        MessageErreur.Text = message;
        _fileName = fileName;
        _replaceMode = import;

        this.Loaded += LoadBtnDyn;
        choiceDyn.Click += CloseErrorOrReplaceOnImport;
        cancel.Click += Leave;
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

    private async void CloseErrorOrReplaceOnExportOrDownload(object? sender, RoutedEventArgs e)
    {
        if (!_replaceMode)
            this.Close();
        else if (_replaceMode)
            this.Hide();
            await Task.Delay(1);
            _ = _libsglobal.ReplaceOnExportOrDownload(_fileName, !_export);
        this.Close();
    }

    private void CloseErrorOrReplaceOnImport(object? sender, RoutedEventArgs e)
    {
        if (_replaceMode)
        {
            _libsglobal.DeleteFile(_libsglobal.GetFileNameWithNoExtention(_fileName));
            _ = _libsglobal.ImportFileFromApi(new ModelDisplayFetchFile() { fileName = _fileName });
        }
        this.Close();
    }

    private void Leave(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

}