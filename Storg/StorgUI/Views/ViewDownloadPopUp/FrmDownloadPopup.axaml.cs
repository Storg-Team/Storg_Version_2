using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StorgCommon;
using StorgLibs;

namespace StorgUI.Views.ViewDownloadPopUp;

public partial class FrmDownloadPopup : Window
{
    private IList<ModelDisplayFiles> _filesList = new List<ModelDisplayFiles>();
    private LibsGlobal _libsglobal = new LibsGlobal();

    public FrmDownloadPopup()
    {
        InitializeComponent();
    }

    public FrmDownloadPopup(IList<ModelDisplayFiles> FilesList) : this()
    {
        _filesList = FilesList;
        LoadingBar.IsVisible = false;

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

    private async void DownloadOptions(object? sender, RoutedEventArgs e)
    {
        if (chkDl.IsChecked != null && chkExp.IsChecked != null)
        {
            if (chkDl.IsChecked.Value && chkExp.IsChecked.Value)
            {
                await Export();
                await Download();
            }
            else if (!chkDl.IsChecked.Value && chkExp.IsChecked.Value)
            {
                await Export();
            }
            else if (chkDl.IsChecked.Value && !chkExp.IsChecked.Value)
            {
                await Download();
            }
        }

        this.Close();
    }

    private async Task Download()
    {
        LoadingBar.IsVisible = true;
        LoadingBar.Value = 0;
        LoadingBar.ProgressTextFormat = "Téléchargement en cours...";
        float gap = 100 / _filesList.Count;
        await Task.Delay(10);
        foreach (ModelDisplayFiles file in _filesList)
        {
            if (_libsglobal.CheckIfExistInDownloadFolder(file.Name))
            {
                FrmErrorPopUp frmErrorPopUp = new FrmErrorPopUp($"Le fichier {file.Name} existe déjà dans le dossier de destination, voulez vous remplacer le fichier existant par ce fichier ?", true, file.Name);
                await frmErrorPopUp.ShowDialog((Window) this.VisualRoot!);
                LoadingBar.Value += gap;
                await Task.Delay(1);
            }   
            else
            {
                if (!await _libsglobal.DownloadFile(file.Name))
                {
                    FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Echec du téléchargement du ficher :" + file.Name);
                    await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
                }
                LoadingBar.Value += gap;
                await Task.Delay(1);
            }
        }

        LoadingBar.ProgressTextFormat = "Terminé";
        await Task.Delay(500);
        LoadingBar.IsVisible = false;
    }

    private async Task Export()
    {
        LoadingBar.IsVisible = true;
        LoadingBar.Value = 0;
        LoadingBar.ProgressTextFormat = "Export en cours...";
        float gap = 100 / _filesList.Count;
        foreach (ModelDisplayFiles file in _filesList)
        {
            if (_libsglobal.CheckIfExistInDownloadFolder(file.Name, false))
            {
                FrmErrorPopUp frmErrorPopUp = new FrmErrorPopUp($"Le dossier Dir_{file.Name} existe déjà dans le dossier de destination, voulez vous remplacer le dossier existant par ce dossier ?", true, file.Name, true);
                await frmErrorPopUp.ShowDialog((Window) this.VisualRoot!);
                LoadingBar.Value += gap;
                await Task.Delay(1);
            }
            else
            {
                if (!await _libsglobal.ExportFile(file.Name))
                {
                    FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Echec de l'export du ficher :" + file.Name);
                    await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
                }
                LoadingBar.Value += gap;
                await Task.Delay(1);
            }
        }

        LoadingBar.ProgressTextFormat = "Terminé";
        await Task.Delay(500);
        LoadingBar.IsVisible = false;
    }


    #endregion Methode
}