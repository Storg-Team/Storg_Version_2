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

namespace StorgUI
{
    public partial class MainWindow : Window
    {

        #region Variable
        private LibsGlobal _libsglobal = new LibsGlobal();
        private string CurrentExecDirectory = "";
        private Control? Page_tmp;
        private static OptionPopUp OptionPopUpWindows = new OptionPopUp(); // Fenetre PopUp Globale pour Exprter / Telecharger / Supprimer un fichier.
        private string _savedFolder;

        #endregion Variable




        public MainWindow()
        {
            InitializeComponent();
            CurrentExecDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _savedFolder = Path.Combine(CurrentExecDirectory, ConfigurationManager.AppSettings.Get("SavedFolder")!);
            refresh(); // Permet d'afficher tout les fichiers d�ja pr�sent dans la BDD

            AddHandler(DragDrop.DropEvent, OnDrop);  //  Ajouter l'�venement pour d�clancher la fonction de Drag and Drop

            this.SizeChanged += Dynamic_Change_Size;  //  Executer la fonction Dynamic_Change_Size d�s que la fenetre change de taille.

            Page_tmp = MainContent.Content as Control;


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

        private void OnClickAccueil(object? sender, RoutedEventArgs e) // Une fois le boutton clicker �a d�clanche les fonctions associer � chaque boutton pour effectuer ce qu'il faut.
        {
            MainContent.Content = Page_tmp;
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

            IEnumerable<IStorageItem>? items = e.Data.GetFiles(); // R�cup�re le ou les objects d�poser
            if (items != null)
            {
                foreach (IStorageFile item in items) // R�cup�re les objets un part un.
                {
                    var file = item as IStorageFile; // R�cup�re le fichier depuis l'objet

                    if (file is null)
                    {
                        return;
                    }

                    Add_File(file); // Le donne � la fonction de cr�ation de fichier.
                }
            }
        }

        private async void OnClickFichierDl(object? sender, RoutedEventArgs e) // Permet d'ouvrir la fenetre PopUp
        {
            if (sender is Button button)
            {
                OptionPopUpWindows = new OptionPopUp(button.Name);
                OptionPopUpWindows.Closed += (s, e) => refresh();  // Quand elle ce ferme on refresh la list des fichiers.
                await OptionPopUpWindows.ShowDialog(this);
            }
        }

        private void Dynamic_Change_Size(object? sender, SizeChangedEventArgs e)  // Permet de changer dynamiquement la taille de la section de scoll en fonction de la taille de l'�cran.
        {
            double WindowHeight = this.Height;
            if (WindowHeight > 283)
            {
                ScollBar.Height = WindowHeight - 283;
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
                await PopUpWindows.ShowDialog(this);
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


        #endregion Methode



















































































        public void Research(object? sender, KeyEventArgs e) // Faire une recherche de fichier
        {
            if (e.Key == Key.Enter) // V�rifi si la touche entrer est presser
            {
                string research_file_text = "";
                if (Search.Text != null) // Si c'est le cas on r�cup�re le texte �crit
                {
                    research_file_text = Search.Text;
                }

                if (research_file_text != "")
                {

                    List<string[]> File_List = new List<string[]>();

                    string exec_py = Path.Combine(Get_Current_Directory(), "Gestion_BDD");

                    if (Os() == "Linux")
                    {
                        exec_py = "/usr/share/storg/Gestion_BDD";

                    }

                    Process process = new Process();
                    process.StartInfo.FileName = exec_py;
                    process.StartInfo.Arguments = $"research_file {research_file_text}";
                    process.StartInfo.RedirectStandardOutput = true;

                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;


                    process.Start();


                    if (process == null)
                    {
                        return;
                    }
                    using (System.IO.StreamReader reader = process.StandardOutput)
                    {

                        string output = reader.ReadToEnd();

                        // Le script python r�cup�re tout les fichier stocker dans la BDD pour les afficher //
                        List<List<string>>? pythonList = JsonConvert.DeserializeObject<List<List<string>>>(output);
                        if (pythonList == null)
                        {
                            return;
                        }

                        foreach (var i in pythonList)
                        {
                            List<string> Element = new List<string>();
                            foreach (var value in i)
                            {
                                Element.Add(value.ToString());
                            }
                            File_List.Add(Element.ToArray());
                        }


                    }



                    File_List.Reverse(); // Inverse la liste pour avoir le fichier le plus r�cent en haut
                    ColumnFichier.Children.Clear(); // Vide la liste afficher.
                    foreach (var i in File_List)
                    {

                        // On recr�� tout les bouttons //

                        var btn = Create_btn(i[0], i[1], i[2], i[3]);

                        btn.Click += OnClickFichierDl; // Affectation de la fonction OnClickFichierDl lors du click

                        ColumnFichier.Children.Add(btn);
                    }


                }

                Focus();
            }
            if (e.Key == Key.Escape) // si escape est presser alors on quitte la zone de recherche
            {
                Focus();
                refresh();
            }
        }















        private async void OpenFileBorwser(object? sender, PointerPressedEventArgs e) // Permet d'ouvrir l'explorateur de fichier
        {

            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.StorageProvider != null)
            {
                var items = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions // R�cup�re le ou les fichiers selectionner
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

        }




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






    }
}


