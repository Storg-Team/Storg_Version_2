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


namespace StorgUI
{
    public partial class ControlMainPage : UserControl
    {

        #region Variable
        private LibsGlobal _libsglobal = new LibsGlobal();

        #endregion Variable


        public ControlMainPage()
        {

            InitializeComponent();

            refresh(); // Permet d'afficher tout les fichiers d�ja pr�sent dans la BDD

            AddHandler(DragDrop.DropEvent, OnDrop);  //  Ajouter l'�venement pour d�clancher la fonction de Drag and Drop

            ContentControl? parent = this.Parent as ContentControl;
            if (parent != null)
            {
                parent.SizeChanged += Dynamic_Change_Size; //  Executer la fonction Dynamic_Change_Size d�s que la fenetre change de taille.
            }


            #region btntrigger

            Button? buttonA = this.FindControl<Button>("BtAccueil"); // Si un boutton avec en parametre le Nom = BtAcueil, alors �a d�clache l'action qui se passe quand on click dessus.
            if (buttonA != null)
            {
                this.Loaded += (sender, e) =>
                {
                    buttonA.Focus();

                };
                buttonA.GotFocus += (sender, e) =>
                {
                    buttonA.Background = new SolidColorBrush(Color.Parse("#bca7a7"));
                };
                buttonA.LostFocus += (sender, e) =>
                {
                    buttonA.Background = new SolidColorBrush(Color.Parse("#d6bebe"));
                };
                buttonA.Click += OnClickAccueil;
            }

            Button? buttonC = this.FindControl<Button>("BtContact");
            if (buttonC != null)
            {
                buttonC.GotFocus += (sender, e) =>
                {
                    buttonC.Background = new SolidColorBrush(Color.Parse("#bca7a7"));
                };
                buttonC.LostFocus += (sender, e) =>
                {
                    buttonC.Background = new SolidColorBrush(Color.Parse("#d6bebe"));
                };
                buttonC.Click += OnClickContact;
            }

            Button? buttonAide = this.FindControl<Button>("BtAide");
            if (buttonAide != null)
            {
                buttonAide.GotFocus += (sender, e) =>
                {
                    buttonAide.Background = new SolidColorBrush(Color.Parse("#bca7a7"));
                };
                buttonAide.LostFocus += (sender, e) =>
                {
                    buttonAide.Background = new SolidColorBrush(Color.Parse("#d6bebe"));
                };
                buttonAide.Click += OnClickAide;
            }

            Button? buttonP = this.FindControl<Button>("BtAProps");
            if (buttonP != null)
            {
                buttonP.GotFocus += (sender, e) =>
                {
                    buttonP.Background = new SolidColorBrush(Color.Parse("#bca7a7"));
                };
                buttonP.LostFocus += (sender, e) =>
                {
                    buttonP.Background = new SolidColorBrush(Color.Parse("#d6bebe"));
                };
                buttonP.Click += OnClickAProps;
            }
            var button = this.FindControl<Button>("Reload");
            if (button != null)
            {
                button.Click += OnClickReload;
            }

            button = this.FindControl<Button>("BtSettings");
            if (button != null)
            {
                button.Click += OnClickSettings;
            }

        }

        #endregion btntrigger





        #region trigger
        private void OnClickSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
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

        private void OnClickContact(object? sender, RoutedEventArgs e)
        {
            Console.WriteLine("Contact");
        }

        private void OnClickAide(object? sender, RoutedEventArgs e)
        {
            MainContent.Content = new ControlNavigationHelper();
        }

        private void OnClickAProps(object? sender, RoutedEventArgs e)
        {
            Console.WriteLine("Aprops");
        }

        private void OnClickReload(object? sender, RoutedEventArgs e) // Appelle la fonction refresh.
        {
            refresh();
        }
        private void OnDrop(object? sender, DragEventArgs e) // Fonction de Drag and Drop
        {

            IEnumerable<IStorageItem>? items = e.Data.GetFiles(); // Recupere le ou les objects deposer
            if (items != null)
            {
                foreach (IStorageFile item in items) // Recupere les objets un part un.
                {
                    var file = item as IStorageFile; // Recupere le fichier depuis l'objet

                    if (file is null)
                    {
                        return;
                    }

                    Add_File(file); // Le donne a la fonction de creation de fichier.
                }
            }
        }

        private async void OnClickFichierDl(object? sender, RoutedEventArgs e) // Permet d'ouvrir la fenetre PopUp
        {
            if (sender is Button button)
            {
                OptionPopUp OptionPopUpWindows = new OptionPopUp(button.Name!);
                OptionPopUpWindows.Closed += (s, e) => refresh();  // Quand elle ce ferme on refresh la list des fichiers.
                await OptionPopUpWindows.ShowDialog((Window) this.VisualRoot!);
            }
        }

        private void Dynamic_Change_Size(object? sender, SizeChangedEventArgs e)  // Permet de changer dynamiquement la taille de la section de scoll en fonction de la taille de l'�cran.
        {
            if (this.Height > 283)
            {
                ScollBar.Height = this.Height - 283;
            }
        }

        #endregion trigger



        #region Methode

        private void refresh()  // Permet de refresh la liste des fichier.
        {
            IList<ModelFile> FilesList = _libsglobal.LoadStoredFile();

            ColumnFichier.Children.Clear(); // Vide la liste afficher.
            foreach (ModelFile file in FilesList)
            {

                var btn = Create_btn(file); //creation dynamique des boutton

                btn.Click += OnClickFichierDl; // Affectation de la fonction OnClickFichierDl lors du click

                ColumnFichier.Children.Add(btn);
            }
        }

        private async void Add_File(IStorageFile file) // Permet de cr�� et d'ajouter un fichier � la BDD
        {

            // R�cup�re les infos importante du fichier (Nom, chemin, taille)

            string NameFile = file.Name.Replace(' ', '_');

            if (_libsglobal.CheckIfFileExist(NameFile))
            {
                FrmErrorPopUp PopUpWindows = new FrmErrorPopUp();
                await PopUpWindows.ShowDialog((Window) this.VisualRoot!);
            }
            else
            {
                using (var stream = await file.OpenReadAsync())
                    _libsglobal.StoreFile(file.Name, file.Path.AbsolutePath, Convert.ToString($"{stream.Length / 1024} Ko"));
            }

            refresh(); // Refresh pour que le nouveau fichier soit en haut

        }

        private Button Create_btn(ModelFile file) // Permet de cr�� un boutton (Utiliser par fonction Ajout file + refresh)
        {

            Button btn = new Button // Cr�ation d'un nouveau boutton
            {
                ///// Param�tre du boutton /////
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 15, 0, 0),
                Height = 30,
                Background = new SolidColorBrush(Color.Parse("#42505f")),
                Name = file.Name,
                /////
            };
            btn.Classes.Add("File");

            Grid grid = new Grid();

            grid.Children.Add(DesignButton(file.Date + " " + file.Time, 420));
            grid.Children.Add(DesignButton(file.Weight, 580));
            grid.Children.Add(DesignButton(file.Name, 10));

            btn.Content = grid;

            return btn;
        }

        private TextBlock DesignButton(string TextValue, int ThickValue)
        {
            return new TextBlock
            {
                Text = TextValue,
                Margin = new Thickness(ThickValue, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
        }

        public void Research(object? sender, KeyEventArgs e) // Faire une recherche de fichier
        {
            if (e.Key == Key.Enter) // Verifi si la touche entrer est presser
            {
                if (Search.Text == null) // Si c'est le cas on recupere le texte ecrit
                {
                    return;
                }
                string research_file_text = Search.Text;

                ColumnFichier.Children.Clear(); // Vide la liste afficher.
                foreach (ModelFile file in _libsglobal.ResearchFileByName(research_file_text).Reverse())
                {
                    // On recree tout les bouttons //

                    Button btn = Create_btn(file);
                    btn.Click += OnClickFichierDl; // Affectation de la fonction OnClickFichierDl lors du click
                    ColumnFichier.Children.Add(btn);
                }
                Focus();
            }
            if (e.Key == Key.Escape) // si escape est presser alors on quitte la zone de recherche
            {
                Focus();
                refresh();
            }
        }

        #endregion Methode



        #region GestionFocusSearch
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
            if (!Search.IsPointerOver)
            {
                Focus();
            }
        }

        #endregion GestionFocusSearch


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
                foreach (var files in items) // Parcourir tout les fichiers selectionner
                {

                    var file = files as IStorageFile;
                    if (file != null)
                    {

                        Add_File(file); // Ajouter les fichier selectionner a la BBD et a la liste
                    }
                }
            }

        }

        #endregion FileBrowser

    }
}