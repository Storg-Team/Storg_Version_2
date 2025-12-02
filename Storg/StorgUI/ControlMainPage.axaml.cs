using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Layout;
using StorgLibs;
using StorgCommon;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using Avalonia.Data;
using System.Security.Cryptography.X509Certificates;
using StorgUI.Views.ViewDownloadPopUp;
using HarfBuzzSharp;
using System.Threading.Tasks;
using Avalonia.Themes.Fluent;


namespace StorgUI
{

    public partial class ControlMainPage : UserControl
    {

        #region Variable
        private LibsGlobal _libsglobal = new LibsGlobal();
        private static bool _isPaneOpen = false;
        private ObservableCollection<ModelDisplayFiles> _dataGridItems = new ObservableCollection<ModelDisplayFiles>();

        #endregion Variable


        public ControlMainPage()
        {

            InitializeComponent();



            LoadingBar.IsVisible = false;
            MainMenu.IsPaneOpen = _isPaneOpen;
            refresh(); // Permet d'afficher tout les fichiers deja present dans la BDD

            FilesGrid.Tapped += DisplayBtnOptionFile;
            AddHandler(DragDrop.DropEvent, OnDrop);  //  Ajouter l'evenement pour declancher la fonction de Drag and Drop

            this.Loaded += SetDynSize;


            #region btntrigger

            string focus = "#bca7a7";
            string lostfocus = "#d6bebe";

            ListBoxItem? listbox = this.FindControl<ListBoxItem>("BtAccueil"); // Si un boutton avec en parametre le Nom = BtAcueil, alors ca declanche l'action qui se passe quand on click dessus.
            if (listbox != null)
            {
                this.Loaded += (sender, e) =>
                {
                    listbox.Focus();

                };
                listbox.GotFocus += (sender, e) =>
                {
                    listbox.Background = new SolidColorBrush(Color.Parse(focus));
                };
                listbox.LostFocus += (sender, e) =>
                {
                    listbox.Background = new SolidColorBrush(Color.Parse(lostfocus));
                };
                listbox.IsSelected = true;
                listbox.Tapped += OnClickAccueil;
                listbox.PointerEntered += ExpendMenuEnter;
                listbox.PointerExited += ExpendMenuLeave;
            }

            listbox = this.FindControl<ListBoxItem>("BtSettings");
            if (listbox != null)
            {
                listbox.GotFocus += (sender, e) =>
                {
                    listbox.Background = new SolidColorBrush(Color.Parse(focus));
                };
                listbox.LostFocus += (sender, e) =>
                {
                    listbox.Background = new SolidColorBrush(Color.Parse(lostfocus));
                };
                listbox.Tapped += OnClickSettings;
                listbox.PointerEntered += ExpendMenuEnter;
                listbox.PointerExited += ExpendMenuLeave;
            }

            listbox = this.FindControl<ListBoxItem>("BtAide");
            if (listbox != null)
            {
                listbox.GotFocus += (sender, e) =>
                {
                    listbox.Background = new SolidColorBrush(Color.Parse(focus));
                };
                listbox.LostFocus += (sender, e) =>
                {
                    listbox.Background = new SolidColorBrush(Color.Parse(lostfocus));
                };
                listbox.Tapped += OnClickAide;
                listbox.PointerEntered += ExpendMenuEnter;
                listbox.PointerExited += ExpendMenuLeave;
            }

            listbox = this.FindControl<ListBoxItem>("BtAProps");
            if (listbox != null)
            {
                listbox.GotFocus += (sender, e) =>
                {
                    listbox.Background = new SolidColorBrush(Color.Parse(focus));
                };
                listbox.LostFocus += (sender, e) =>
                {
                    listbox.Background = new SolidColorBrush(Color.Parse(lostfocus));
                };
                listbox.Tapped += OnClickAProps;
                listbox.PointerEntered += ExpendMenuEnter;
                listbox.PointerExited += ExpendMenuLeave;
            }

            Button? button = this.FindControl<Button>("Reload");
            if (button != null)
            {
                button.Click += OnClickReload;
            }
            button = this.FindControl<Button>("BtResearch");
            if (button != null)
            {
                button.Click += TriggerClickResearch;
            }

            button = this.FindControl<Button>("Telecharger");
            if (button != null)
            {
                button.Click += OnclickDl;
            }

            button = this.FindControl<Button>("Supprimer");
            if (button != null)
            {
                button.Click += OnclickDel;
            }

        }


        #endregion btntrigger





        #region trigger
        private void OnClickSettings(object? sender, RoutedEventArgs e)
        {
            MainContent.Content = new ControlSettings();
        }

        private void OnClickAccueil(object? sender, RoutedEventArgs e) // Une fois le boutton clicker ca declanche les fonctions associer a chaque boutton pour effectuer ce qu'il faut.
        {
            ContentControl? parent = this.Parent as ContentControl;
            if (parent != null)
            {
                parent.Content = new ControlMainPage();
            }

        }

        private void OnClickAide(object? sender, RoutedEventArgs e)
        {
            MainContent.Content = new ControlNavigationHelper();
        }

        private void OnClickAProps(object? sender, RoutedEventArgs e)
        {
            MainContent.Content = new FrmAPropos();
        }

        private void OnClickReload(object? sender, RoutedEventArgs e) // Appel la fonction refresh.
        {
            Search.Text = "Rechercher des fichiers compressés";
            refresh();
        }

        private void OnclickDel(object? sender, RoutedEventArgs e)
        {
            Delete();
            Telecharger.IsVisible = false;
            Supprimer.IsVisible = false;
        }

        private void OnclickDl(object? sender, RoutedEventArgs e)
        {
            Download();
            Telecharger.IsVisible = false;
            Supprimer.IsVisible = false;
        }

        private async Task OnDrop(object? sender, DragEventArgs e) // Fonction de Drag and Drop
        {

            IReadOnlyList<IStorageFile>? items = (IReadOnlyList<IStorageFile>?)e.Data.GetFiles(); // Recupere le ou les objects deposer
            if (items != null)
            {
                await LoopOnFileToAdd(items);
            }
        }

        private void DisplayBtnOptionFile(object? sender, RoutedEventArgs e)
        {
            if (FilesGrid.SelectedItems.Count != 0)
            {
                Telecharger.IsVisible = true;
                Supprimer.IsVisible = true;
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

        private void Dynamic_Change_Size(object? sender, SizeChangedEventArgs e)  // Permet de changer dynamiquement la taille de la section de scoll en fonction de la taille de l'�cran.
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
                FilesGrid.Height = parent.Height - 275;
            }
        }

        #endregion WindowsDynamicSize




        #region Methode

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

            _dataGridItems = new ObservableCollection<ModelDisplayFiles>(this.CastModelFile(_libsglobal.LoadStoredFile()));
            FilesGrid.ItemsSource = _dataGridItems;

            Telecharger.IsVisible = false;
            Supprimer.IsVisible = false;
        }

        private async void Add_File(IStorageFile file, float gap) // Permet de cree et d'ajouter un fichier a la BDD
        {

            string FileWeight = "";

            if (_libsglobal.CheckIfFileExistInBDD(file.Name))
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
                    if (!await _libsglobal.StoreFile(file.Name, file.TryGetLocalPath()!, FileWeight))
                    {
                        FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Import du fichier impossible");
                        await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
                    }
                }
                else
                {
                    FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Import du fichier impossible");
                    await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
                }
                _dataGridItems.Add(new ModelDisplayFiles() { Name = file.Name, Date = _libsglobal.GetDateTime().Date + " " + _libsglobal.GetDateTime().Time, Weight = FileWeight });
                LoadingBar.Value += gap;
            }
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

        private void Research()
        {
            if (Search.Text == null) // Si c'est le cas on recupere le texte ecrit
            {
                return;
            }
            string research_file_text = Search.Text;

            FilesGrid.ItemsSource = new ObservableCollection<ModelDisplayFiles>(this.CastModelFile(_libsglobal.ResearchFileByName(research_file_text)).Reverse());

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
            IList<ModelDisplayFiles> FilesList = FilesGrid.SelectedItems.Cast<ModelDisplayFiles>().ToList();
            foreach (ModelDisplayFiles file in FilesList)
            {
                if (!_libsglobal.DeleteFile(file.Name))
                {
                    FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Echec de le suppression du ficher :" + file.Name);
                    await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
                }
            }
            this.refresh();

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
                Telecharger.IsVisible = false;
                Supprimer.IsVisible = false;
            }
        }

        #endregion GestionFocus


        #region FileBrowser

        private async void OpenFileBorwser(object? sender, PointerPressedEventArgs e) // Permet d'ouvrir l'explorateur de fichier
        {
            TopLevel topLevel = TopLevel.GetTopLevel(this)!;
            if (topLevel.StorageProvider == null)
            {
                return;
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

            if (items.Count > 0)
            {
                LoadingBar.ProgressTextFormat = "Import en cours...";
                LoadingBar.Value = 0;
                LoadingBar.IsVisible = true;
                await Task.Delay(10);
                await LoopOnFileToAdd(items);

                LoadingBar.ProgressTextFormat = "Terminé";
                await Task.Delay(500);
                LoadingBar.IsVisible = false;

            }

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

        #endregion FileBrowser

    }

}
