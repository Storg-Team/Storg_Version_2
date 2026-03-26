using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using StorgLibs;
using StorgCommon;
using System.Linq;
using System.Collections.ObjectModel;
using Avalonia.Data;
using StorgUI.Views.ViewDownloadPopUp;
using System.Threading.Tasks;
using StorgUI.Views.ViewFetchFiles;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System.Runtime.CompilerServices;
using System.IO;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia;


namespace StorgUI
{

    public partial class ControlMainPage : UserControl
    {

        #region Variable
        private LibsGlobal _libsGlobal = new LibsGlobal();
        private static bool _isPaneOpen = false;
        private ModelSettings _settings = new ModelSettings();
        private ObservableCollection<ModelDisplayFiles> _dataGridItems = new ObservableCollection<ModelDisplayFiles>();

        #endregion Variable


        public ControlMainPage()
        {


            InitializeComponent();

            this.Loaded += OnLoadSettings;
            this.Loaded += SetDynSize;
            UpdateIcons();
            if (Application.Current != null)
                Application.Current.ActualThemeVariantChanged += (s, e) => UpdateIcons();

            LoadingBar.IsVisible = false;
            MainMenu.IsPaneOpen = _isPaneOpen;
            this.refresh();

            FilesGrid.Tapped += DisplayBtnOptionFile;
            dragDrop.AddHandler(DragDrop.DropEvent, OnDrop);  //  Ajouter l'evenement pour declencher la fonction de Drag and Drop



            #region btntrigger


            // Set les events des boutons
            // btnAcceuil
            BtAccueil.Focus();
            BtAccueil.GotFocus += SetFocusStyle;
            BtAccueil.LostFocus += SetFocusStyle;
            BtAccueil.IsSelected = true;
            BtAccueil.Tapped += OnClickAccueil;
            BtAccueil.PointerEntered += ExpendMenuEnter;
            BtAccueil.PointerExited += ExpendMenuLeave;
            BtAccueil.KeyDown += OnKeyDownAcceuil;

            //btnSettings
            BtSettings.GotFocus += SetFocusStyle;
            BtSettings.LostFocus += SetDefaultStyle;
            BtSettings.Tapped += OnClickSettings;
            BtSettings.PointerEntered += ExpendMenuEnter;
            BtSettings.PointerExited += ExpendMenuLeave;
            BtSettings.KeyDown += OnKeyDownSettings;

            //btnAide
            BtAide.GotFocus += SetFocusStyle;
            BtAide.LostFocus += SetDefaultStyle;
            BtAide.Tapped += OnClickAide;
            BtAide.PointerEntered += ExpendMenuEnter;
            BtAide.PointerExited += ExpendMenuLeave;
            BtAide.KeyDown += OnKeyDownAide;

            //btnAprops
            BtAProps.GotFocus += SetFocusStyle;
            BtAProps.LostFocus += SetDefaultStyle;
            BtAProps.Tapped += OnClickAProps;
            BtAProps.PointerEntered += ExpendMenuEnter;
            BtAProps.PointerExited += ExpendMenuLeave;
            BtAProps.KeyDown += OnKeyDownAProps;

            //btnLiveDecomp
            BtLiveDecomp.PointerEntered += ExpendMenuEnter;
            BtLiveDecomp.PointerExited += ExpendMenuLeave;
            BtLiveDecomp.Tapped += OnClickLiveDecompression;
            BtLiveDecomp.KeyDown += OnKeyDownLiveDecompression;

            btnReload.Click += OnClickReload;
            BtResearch.Click += TriggerClickResearch;
            btnTelecharger.Click += OnclickDl;
            btnSupprimer.Click += OnclickDel;
            btnFetch.Click += OnClickFetchFiles;
            btnUpload.Click += OnClickUpload;
            folderImport.PointerPressed += OnClickOpenFolderBrowser;

            #endregion btntrigger
        }






        #region trigger

        private void OnLoadSettings(object? sender, RoutedEventArgs e)
        {
            _settings = _libsGlobal.LoadSettings();
            btnFetch.IsVisible = _settings.isConnected;

            Application.Current!.RequestedThemeVariant = _settings.lightMode ? ThemeVariant.Light : ThemeVariant.Dark;
        }

        private void OnClickSettings(object? sender, RoutedEventArgs e)
        {
            this.LoadFrmSettings();
        }

        private void UpdateIcons()
        {
            var isDark = Application.Current?.ActualThemeVariant == ThemeVariant.Dark;
            var folder = isDark ? "Dark" : "Light";

            SetIcon("ImgAccueil", $"avares://StorgUI/Resources/{folder}/logo_accueil.png");
            SetIcon("ImgSettings", $"avares://StorgUI/Resources/{folder}/settings.png");
            SetIcon("ImgHelp", $"avares://StorgUI/Resources/{folder}/logo_help.png");
            SetIcon("ImgAPropos", $"avares://StorgUI/Resources/{folder}/logo_a_propos.png");
            SetIcon("FolderInput", $"avares://StorgUI/Resources/{folder}/folder-input.png");
            SetIcon("ImgSearch", $"avares://StorgUI/Resources/{folder}/search.png");
            SetIcon("Reload", $"avares://StorgUI/Resources/{folder}/reload.png");
            SetIcon("ImgDownload", $"avares://StorgUI/Resources/{folder}/download.png");
            SetIcon("Trash", $"avares://StorgUI/Resources/{folder}/trash-2.png");
            SetIcon("HardDriveUpload", $"avares://StorgUI/Resources/{folder}/hard-drive-upload.png");
            SetIcon("CloudBackup", $"avares://StorgUI/Resources/{folder}/cloud-backup.png");
        }

        private void SetIcon(string name, string uri)
        {
            var img = this.FindControl<Image>(name);
            if (img == null) return;
            try
            {
                var stream = AssetLoader.Open(new Uri(uri));
                img.Source = new Bitmap(stream);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SetIcon] Erreur pour {name} : {e.Message}");
            }
        }

        private void OnKeyDownSettings(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.LoadFrmSettings();
            }
        }

        private void OnClickAccueil(object? sender, RoutedEventArgs e) // Une fois le boutton clicker ca declanche les fonctions associer a chaque boutton pour effectuer ce qu'il faut.
        {
            this.LoadFrmAcceuil();
        }

        private void OnKeyDownAcceuil(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.LoadFrmAcceuil();
            }
        }

        private void OnClickAide(object? sender, RoutedEventArgs e)
        {
            this.LoadFrmAide();
        }

        private void OnKeyDownAide(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.LoadFrmAide();
            }
        }

        private void OnClickAProps(object? sender, RoutedEventArgs e)
        {
            this.LoadFrmAProps();
        }

        private void OnKeyDownAProps(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.LoadFrmAProps();
            }
        }

        private void OnClickOpenFolderBrowser(object? sender, PointerPressedEventArgs e)
        {
            this.OpenFolderBrowser();
        }

        private void OnKeyDownOpenFolderBrowser(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.OpenFolderBrowser();
            }
        }

        private async void OnClickLiveDecompression(object? sender, TappedEventArgs e)
        {
            this.LiveDecompression();
        }

        private async void OnKeyDownLiveDecompression(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.LiveDecompression();
            }
        }

        private void OnClickReload(object? sender, RoutedEventArgs e) // Appel la fonction refresh.
        {
            Search.Text = "Rechercher des fichiers compressés";
            refresh();
        }

        private void OnclickDel(object? sender, RoutedEventArgs e)
        {
            Delete();
            this.SetVisibility();
        }

        private void OnclickDl(object? sender, RoutedEventArgs e)
        {
            Download();
            this.SetVisibility();

        }

        private async Task OnDrop(object? sender, DragEventArgs e) // Fonction de Drag and Drop
        {
            if (e.Data.Contains("text/uri-list")) dragDrop.Background = Brushes.Red;
            if (e.Data.Contains(DataFormats.Files))
            {
                IReadOnlyList<IStorageFile>? items = (IReadOnlyList<IStorageFile>?)e.Data.GetFiles(); // Recupere le ou les objects deposer
                if (items != null)
                {
                    await LoopOnFileToAdd(items);
                }
            }
        }

        private void DisplayBtnOptionFile(object? sender, RoutedEventArgs e)
        {
            if (FilesGrid.SelectedItems.Count != 0)
            {
                btnTelecharger.IsVisible = true;
                btnSupprimer.IsVisible = true;
                btnUpload.IsVisible = _settings.isConnected;
            }
        }

        private void TriggerClickResearch(object? sender, RoutedEventArgs e)
        {
            Research();
        }

        private void ExpendMenuEnter(object? sender, RoutedEventArgs e)
        {
            MainMenu.IsPaneOpen = true;
            _isPaneOpen = MainMenu.IsPaneOpen;
        }

        private void ExpendMenuLeave(object? sender, RoutedEventArgs e)
        {
            MainMenu.IsPaneOpen = false;
            _isPaneOpen = MainMenu.IsPaneOpen;
        }

        private void OnClickFetchFiles(object? sender, RoutedEventArgs e)
        {
            this.FetchFiles();
        }

        private async void OnClickUpload(object? sender, RoutedEventArgs e)
        {
            LoadingBar.ProgressTextFormat = "Transfet en cours...";
            LoadingBar.Value = 0;
            LoadingBar.IsVisible = true;
            await Task.Delay(10);
            await this.Upload();
            LoadingBar.ProgressTextFormat = "Transfet terminé";
            await Task.Delay(500);
            LoadingBar.IsVisible = false;
            LoadingBar.Value = 0;

            this.SetVisibility();
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            this.refresh();
        }

        public void TriggerKeyResearch(object? sender, KeyEventArgs e) // Faire une recherche de fichier
        {
            if (e.Key == Key.Enter) // Verifi si la touche entrer est presser
            {
                Research();
            }
            if (e.Key == Key.Escape) // si escape est presser alors on quitte la zone de recherche
            {
                Focus();
                refresh();
            }
        }


        #endregion trigger

        #region WindowsDynamicSize

        private void SetDynSize(object? sender, RoutedEventArgs e)
        {
            if (this.Parent as ContentControl != null && this.Parent.Parent as ContentControl != null)
            {
                ContentControl? parent = this.Parent.Parent as ContentControl;
                parent!.SizeChanged += Dynamic_Change_Size; //  Executer la fonction Dynamic_Change_Size quand la fenetre change de taille.
                SizeDyn(parent!);
            }
        }

        private void Dynamic_Change_Size(object? sender, SizeChangedEventArgs e)  // Permet de changer dynamiquement la taille de la section de scoll en fonction de la taille de l'ecran.
        {
            if (this.Parent as ContentControl != null && this.Parent.Parent as ContentControl != null)
            {
                ContentControl? parent = this.Parent.Parent as ContentControl;
                SizeDyn(parent!);
            }
        }

        private void SizeDyn(ContentControl parent)
        {
            if (parent!.Height > 283)
            {
                FilesGrid.Height = parent.Height - 277;
            }
        }

        #endregion WindowsDynamicSize




        #region Methode

        private void LoadFrmAcceuil()
        {
            ContentControl? parent = this.Parent as ContentControl;
            if (parent != null)
            {
                parent.Content = new ControlMainPage();
            }
        }

        private void LoadFrmSettings()
        {
            MainContent.Content = new ControlSettings();
        }

        private void LoadFrmAide()
        {
            MainContent.Content = new ControlNavigationHelper();
        }

        private void LoadFrmAProps()
        {
            MainContent.Content = new FrmAPropos();
        }

        private async void LiveDecompression()
        {
            IReadOnlyList<IStorageFile> items = await OpenFileBorwser();

            if (items.Count > 0)
            {
                LiveDecompressionWithProgressBar(items);
            }
        }

        private void SetVisibility()
        {
            btnTelecharger.IsVisible = false;
            btnSupprimer.IsVisible = false;
            btnUpload.IsVisible = false;
        }

        private void SetFocusStyle(object? sender, RoutedEventArgs e)
        {
            // string focus = "#546873";
            // this.Background = new SolidColorBrush(Color.Parse(focus));
        }

        private void SetDefaultStyle(object? sender, RoutedEventArgs e)
        {
            // string lostfocus = "#879DA9";
            // this.Background = new SolidColorBrush(Color.Parse(lostfocus));
        }


        private void InitDataGridFiles()
        {
            DataGridTextColumn colName = new DataGridTextColumn()
            {
                Header = "Nom",
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                CanUserResize = true,
                MinWidth = 100,
                Binding = new Binding("Name"),
            };
            DataGridTextColumn colDate = new DataGridTextColumn()
            {
                Header = "Date et Heure",
                CanUserResize = true,
                MinWidth = 100,
                Binding = new Binding("Date"),
            };
            DataGridTextColumn colWeight = new DataGridTextColumn()
            {
                Header = "Taille",
                CanUserResize = true,
                MinWidth = 100,
                Binding = new Binding("Weight"),
            };

            FilesGrid.Columns.Add(colName);
            FilesGrid.Columns.Add(colDate);
            FilesGrid.Columns.Add(colWeight);
        }

        private IEnumerable<ModelDisplayFiles> CastModelFile(IList<ModelFile> FilesList)
        {
            IEnumerable<ModelDisplayFiles> displayFiles = from file in FilesList select new ModelDisplayFiles() { Name = file.Name, Date = file.Date + " " + file.Time, Weight = file.Weight };

            return displayFiles;
        }

        private void refresh()  // Permet de refresh la liste des fichier.
        {

            FilesGrid.Columns.Clear(); // Vide la liste afficher.

            this.InitDataGridFiles();

            _dataGridItems = new ObservableCollection<ModelDisplayFiles>(this.CastModelFile(_libsGlobal.LoadStoredFile()));
            FilesGrid.ItemsSource = _dataGridItems;

            btnTelecharger.IsVisible = false;
            btnSupprimer.IsVisible = false;
            btnUpload.IsVisible = false;
        }

        private async void Add_File(IStorageFile file, float gap) // Permet de cree et d'ajouter un fichier a la BDD
        {

            string FileWeight = "";

            if (_libsGlobal.CheckIfFileExistInBDD(file.Name))
            {
                FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Fichier déja existant");
                await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
            }
            else
            {
                using (var stream = await file.OpenReadAsync())
                    FileWeight = Convert.ToString($"{stream.Length / 1024} Ko");
                if (file.TryGetLocalPath() != null)
                {
                    if (!await _libsGlobal.StoreFile(file.Name, file.TryGetLocalPath()!, FileWeight))
                    {
                        FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Import du fichier impossible");
                        await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
                    }
                    else
                    {
                        _dataGridItems.Add(new ModelDisplayFiles() { Name = file.Name, Date = _libsGlobal.GetDateTime().Date + " " + _libsGlobal.GetDateTime().Time, Weight = FileWeight });
                    }
                }
                else
                {
                    FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Import du fichier impossible");
                    await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
                }
                LoadingBar.Value += gap;
            }
        }

        private void Research()
        {
            if (Search.Text == null) // Si c'est le cas on recupere le texte ecrit
            {
                return;
            }
            string research_file_text = Search.Text;

            FilesGrid.ItemsSource = new ObservableCollection<ModelDisplayFiles>(this.CastModelFile(_libsGlobal.ResearchFileByName(research_file_text)).Reverse());

            Focus();
        }

        private async void Download() // Re telecharger le fichier telle qu'il etait
        {

            IList<ModelDisplayFiles> FilesList = FilesGrid.SelectedItems.Cast<ModelDisplayFiles>().ToList();

            FrmDownloadPopup frmDownloadPopup = new FrmDownloadPopup(FilesList);
            await frmDownloadPopup.ShowDialog((Window)this.VisualRoot!);

            this.refresh();
        }

        private async void Delete() // Permet de supprimer un fichier
        {
            IList<ModelDisplayFiles> files = FilesGrid.SelectedItems.Cast<ModelDisplayFiles>().ToList();
            foreach (ModelDisplayFiles file in files)
            {
                if (!_libsGlobal.DeleteFile(file.Name))
                {
                    FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Echec de le suppression du ficher :" + file.Name);
                    await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
                }
            }
            this.refresh();

        }

        private async Task Upload()
        {
            IList<ModelDisplayFiles> files = FilesGrid.SelectedItems.Cast<ModelDisplayFiles>().ToList();
            float gap = 100.0f / files.Count;
            foreach (ModelDisplayFiles file in files)
            {
                if (!await _libsGlobal.UploadFileFromApi(file))
                {
                    FrmErrorPopUp frmErrorPopUp = new FrmErrorPopUp($"Impossible d'uploader le fichier : {file.Name}\nIl est possible que le fichier existe déjà ou que le serveur rencontre un problème.");
                    await frmErrorPopUp.ShowDialog((Window)this.VisualRoot!);
                }
                LoadingBar.Value += gap;
                await Task.Delay(1);
            }
        }

        private void FetchFiles()
        {
            FrmFetchFiles fetchFiles = new FrmFetchFiles();
            fetchFiles.Show((Window)this.VisualRoot!);
            fetchFiles.Closed += OnClosed;
        }




        #endregion Methode



        #region GestionFocus
        private void Focus(object sender, RoutedEventArgs e) // Permet de vider le texte de la TextBox quand on click dedans
        {
            if (Search.Text == "Rechercher des fichiers compressés")
            {
                Search.Text = string.Empty;
            }
        }

        private void NoFocus(object sender, RoutedEventArgs e) // Permet de remetre le text de la Textbox quand on la quitte si elle est vide
        {
            if (string.IsNullOrWhiteSpace(Search.Text))
            {
                Search.Text = "Rechercher des fichiers compressés";
            }
        }

        private void Lost_Focus(object sender, PointerPressedEventArgs e) // Si on click a coter alors on quitte la TextBox
        {
            // Gestion du focus de la barre de recherche
            if (!Search.IsPointerOver)
            {
                Focus();
            }

            //Gestion du focus de la datagrid au click sur le MainControl
            if (!FilesGrid.IsKeyboardFocusWithin)
            {
                FilesGrid.SelectedItems.Clear();
                btnTelecharger.IsVisible = false;
                btnSupprimer.IsVisible = false;
                btnUpload.IsVisible = false;
            }
        }

        #endregion GestionFocus


        #region FileBrowser

        private async void FileBrowser(object? sender, PointerPressedEventArgs e)
        {
            IReadOnlyList<IStorageFile> items = await OpenFileBorwser();

            if (items.Count > 0)
            {
                AddWithProgressBar(items);
            }
        }

        private async Task<IReadOnlyList<IStorageFile>> OpenFileBorwser() // Permet d'ouvrir l'explorateur de fichier
        {
            TopLevel topLevel = TopLevel.GetTopLevel(this)!;
            if (topLevel.StorageProvider == null)
            {
                return [];
            }

            IReadOnlyList<IStorageFile> items = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions // Recupere le ou les fichiers selectionner
            {
                Title = "Sélection votre fichier", // Titre de la fenetre
                AllowMultiple = true,  // Permet de selectionner plusieur fichier
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new("Tous les fichiers") { Patterns = new[] {"*"} }, // Permet de choisir le type de fichier
                }
            });

            return items;
        }

        private async void OpenFolderBrowser() // Permet d'ouvrir l'explorateur de fichier
        {
            TopLevel topLevel = TopLevel.GetTopLevel(this)!;
            if (topLevel.StorageProvider == null)
            {
                return;
            }

            IReadOnlyList<IStorageFolder> folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions // Recupere le ou les fichiers selectionner
            {
                Title = "Sélection votre fichier", // Titre de la fenetre
                AllowMultiple = true,  // Permet de selectionner plusieur fichier
            });

            if (folders.Count > 0)
            {
                foreach (IStorageFolder folder in folders)
                {
                    List<IStorageFile> files = new List<IStorageFile>();
                    await foreach (IStorageItem item in folder.GetItemsAsync())
                    {
                        if (item is IStorageFile) files.Add((IStorageFile)item);
                    }

                    AddWithProgressBar(files);
                }
            }
        }

        private async void AddWithProgressBar(IReadOnlyList<IStorageFile> files)
        {
            LoadingBar.ProgressTextFormat = "Import en cours...";
            LoadingBar.Value = 0;
            LoadingBar.IsVisible = true;
            await Task.Delay(10);

            await LoopOnFileToAdd(files);

            LoadingBar.ProgressTextFormat = "Terminé";
            await Task.Delay(500);
            LoadingBar.IsVisible = false;
        }

        private async Task LoopOnFileToAdd(IReadOnlyList<IStorageFile> items)
        {
            float gap = 100.0f / items.Count;
            foreach (var files in items) // Parcourir tout les fichiers selectionner
            {

                var file = files as IStorageFile;
                if (file != null)
                {

                    Add_File(file, gap); // Ajouter les fichier selectionner a la BBD et a la liste
                    await Task.Delay(1);

                }
            }
            // LoadingBar.IsVisible = false;
            refresh(); // Refresh pour que le nouveau fichier soit en haut
        }

        private async void LiveDecompressionWithProgressBar(IReadOnlyList<IStorageFile> files)
        {
            LoadingBar.ProgressTextFormat = "Decompression en cours...";
            LoadingBar.Value = 0;
            LoadingBar.IsVisible = true;
            await Task.Delay(10);

            await LoopOnLiveDecompression(files);

            LoadingBar.ProgressTextFormat = "Terminé";
            await Task.Delay(500);
            LoadingBar.IsVisible = false;
        }

        private async Task LoopOnLiveDecompression(IReadOnlyList<IStorageFile> items)
        {
            float gap = 100.0f / items.Count;
            foreach (var files in items)
            {

                var file = files as IStorageFile;
                if (file != null)
                {
                    string filePath = file.TryGetLocalPath()!;
                    FrmErrorPopUp frmErrorPopUp = new FrmErrorPopUp($"Le fichier {Path.GetFileName(filePath)} ne peut pas être décompresser");
                    if (Path.GetExtension(filePath) == ".zip")
                    {
                        if (!await _libsGlobal.LiveDecompression(filePath))
                        {
                            await frmErrorPopUp.ShowDialog((Window)this.VisualRoot!);
                        }
                    }
                    else
                    {
                        await frmErrorPopUp.ShowDialog((Window)this.VisualRoot!);
                    }
                    LoadingBar.Value += gap;
                    await Task.Delay(1);
                }
            }
        }


        #endregion FileBrowser

    }

}
