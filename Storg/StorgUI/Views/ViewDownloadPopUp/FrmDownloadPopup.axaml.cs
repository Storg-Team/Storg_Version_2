using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StorgCommon;
using StorgLibs;

namespace StorgUI.Views.ViewDownloadPopUp;

public partial class FrmDownloadPopup : Window
{
    private IList<ModelDisplayFiles> _filesList;
    private LibsGlobal _libsglobal = new LibsGlobal();

    public FrmDownloadPopup(IList<ModelDisplayFiles> FilesList)
    {
        InitializeComponent();
        _filesList = FilesList;

        #region btntrigger

        Button? button = this.FindControl<Button>("BtnCancel");
        if (button != null)
        {
            button.Click += CloseWindowsOnCancel;
        }

        button = this.FindControl<Button>("BtnProcced");
        if (button != null)
        {
            button.Click += DownloadOptions;
        }


        #endregion btntrigger
    }

    #region  Methode

    private void CloseWindowsOnCancel(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void DownloadOptions(object? sender, RoutedEventArgs e)
    {
        if (chkDl.IsChecked != null && chkExp.IsChecked != null)
        {
            if (chkDl.IsChecked.Value && chkExp.IsChecked.Value)
            {
                Export();
                Download();
            }else if (!chkDl.IsChecked.Value && chkExp.IsChecked.Value)
            {
                Export();
            }else if (chkDl.IsChecked.Value && !chkExp.IsChecked.Value)
            {
                Download();
            }
        }

        this.Close();
    }

    private async void Download()
    {
        foreach (ModelDisplayFiles file in _filesList)
        {
            if (!_libsglobal.DownloadFile(file.Name))
            {
                FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Echec du téléchargement du ficher :" + file.Name);
                await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
            }
        }
    }

    private async void Export()
    {
        foreach (ModelDisplayFiles file in _filesList)
        {
            if (!_libsglobal.ExportFile(file.Name))
            {
                FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Echec de l'export du ficher :" + file.Name);
                await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
            }
        }
    }


    #endregion Methode
}