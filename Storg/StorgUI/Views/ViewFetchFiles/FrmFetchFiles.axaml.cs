using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        btnImporter.Click += Importer;
    }

    #region Methode

    private void UnSelectDataGrid(object? sender, RoutedEventArgs e)
    {
        if (!dataGrid.IsKeyboardFocusWithin) dataGrid.SelectedItems.Clear();
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
        return from file in filesList select new ModelDisplayFetchFile(){fileName = file};
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

    private async void Importer(object? sender, RoutedEventArgs e)
    {

        IList<ModelDisplayFetchFile> fileNameList = dataGrid.SelectedItems.Cast<ModelDisplayFetchFile>().ToList();

        await _libsGlobal.ImportFileFromApi(fileNameList);

        this.Close();
    }


    #endregion Methode
}