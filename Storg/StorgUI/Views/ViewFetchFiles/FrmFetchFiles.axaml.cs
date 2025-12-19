using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using StorgCommon;
using StorgLibs;

namespace StorgUI.Views.ViewFetchFiles;

public partial class FrmFetchFiles : Window
{

    private LibsGlobal _libsGlobal = new LibsGlobal();

    public FrmFetchFiles()
    {
        InitializeComponent();

        this.Loaded += LoadFiles;
        btnImporter.Click += OnClickImport;
        btnSupprimer.Click += OnClickDelete;
        dataGrid.SelectionChanged += DisplayDelete;
    }

    #region Trigger

    private async void OnClickImport(object? sender, RoutedEventArgs e)
    {
        loadingBar.Value = 0;
        loadingBar.ProgressTextFormat = "Import en cours...";
        loadingBar.IsVisible = true;
        await Task.Delay(10);
        await this.Importer();

        loadingBar.ProgressTextFormat = "Import terminé";
        await Task.Delay(500);
        loadingBar.IsVisible = false;
        loadingBar.Value = 0;
        this.Close();
    }

    private void DisplayDelete(object? sender, RoutedEventArgs e)
    {
        btnSupprimer.IsVisible = true;
    }

    private async void OnClickDelete(object? sender, RoutedEventArgs e)
    {
        loadingBar.Value = 0;
        loadingBar.ProgressTextFormat = "Suppression en cours...";
        loadingBar.IsVisible = true;
        await Task.Delay(10);
        await this.Delete();

        loadingBar.ProgressTextFormat = "Suppression terminé";
        await Task.Delay(500);
        loadingBar.IsVisible = false;
        loadingBar.Value = 0;
        this.Close();
    }

    #endregion Trigger



    #region Methode

    private void UnSelectDataGrid(object? sender, RoutedEventArgs e)
    {
        if (!dataGrid.IsKeyboardFocusWithin)
        {
            dataGrid.SelectedItems.Clear();
            btnSupprimer.IsVisible = false;
        }
    }

    private async void LoadFiles(object? sender, RoutedEventArgs e)
    {
        IList<string> filesList = await _libsGlobal.GetFilesUploaded();
        dataGrid.Columns.Clear();
        this.SetDataGridColumn();
        dataGrid.ItemsSource = new ObservableCollection<ModelDisplayFetchFile>(this.CastDisplayFile(filesList));
    }

    private IEnumerable<ModelDisplayFetchFile> CastDisplayFile(IList<string> filesList)
    {
        return from file in filesList select new ModelDisplayFetchFile() { fileName = file };
    }

    private void SetDataGridColumn()
    {
        DataGridTextColumn column = new DataGridTextColumn()
        {
            Header = "Nom du fichier",
            Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            Binding = new Binding("fileName"),
            Foreground = Brushes.Black,
        };

        dataGrid.Columns.Add(column);
    }

    private async Task Importer()
    {
        IList<ModelDisplayFetchFile> fileNameList = dataGrid.SelectedItems.Cast<ModelDisplayFetchFile>().ToList();

        float gap = 100.0f / fileNameList.Count;
        foreach (ModelDisplayFetchFile fileName in fileNameList)
        {
            if (!await _libsGlobal.ImportFileFromApi(fileName))
            {
                FrmErrorPopUp frmErrorPopUp = new FrmErrorPopUp($"Impossible d'importer le fichier : {fileName.fileName}", fileName.fileName, true);
                await frmErrorPopUp.ShowDialog((Window)this.VisualRoot!);
            }
            loadingBar.Value += gap;
            await Task.Delay(1);
        }
    }

    private async Task Delete()
    {
        IList<ModelDisplayFetchFile> fileNameList = dataGrid.SelectedItems.Cast<ModelDisplayFetchFile>().ToList();

        float gap = 100.0f / fileNameList.Count;
        foreach (ModelDisplayFetchFile fileName in fileNameList)
        {
            if (!await _libsGlobal.DeleteFileApi(fileName.fileName))
            {
                FrmErrorPopUp frmErrorPopUp = new FrmErrorPopUp($"Impossible de supprimer le fichier : {fileName.fileName}");
                await frmErrorPopUp.ShowDialog((Window)this.VisualRoot!);
            }
            loadingBar.Value += gap;
            await Task.Delay(1);
        }
    }


    #endregion Methode
}