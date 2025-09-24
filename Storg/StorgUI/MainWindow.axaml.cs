using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using System;
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

        private LibsGlobal _libsglobal = new LibsGlobal();
        private string CurrentExecDirectory = "";
        private Control? Page_tmp;
        public static OptionPopUp OptionPopUpWindows = new OptionPopUp(); // Fenetre PopUp Globale pour Exprter / Telecharger / Supprimer un fichier.


        public MainWindow()
        {
            InitializeComponent();
            CurrentExecDirectory = AppDomain.CurrentDomain.BaseDirectory;
            refresh(); // Permet d'afficher tout les fichiers d�ja pr�sent dans la BDD

            AddHandler(DragDrop.DropEvent, OnDrop);  //  Ajouter l'�venement pour d�clancher la fonction de Drag and Drop

            this.SizeChanged += Dynamic_Change_Size;  //  Executer la fonction Dynamic_Change_Size d�s que la fenetre change de taille.

            Page_tmp = MainContent.Content as Control;


            #region declencheur

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

        #endregion declencheur

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

        #endregion trigger

        private void Dynamic_Change_Size(object? sender, SizeChangedEventArgs e)  // Permet de changer dynamiquement la taille de la section de scoll en fonction de la taille de l'�cran.
        {
            double WindowHeight = this.Height;
            if (WindowHeight > 283)
            {
                ScollBar.Height = WindowHeight - 283;
            }
        }


        #region Methode

        private void refresh()  // Permet de refresh la liste des fichier.
        {
            IList<ModelFile> FilesList = _libsglobal.LoadStoredFile();

            ColumnFichier.Children.Clear(); // Vide la liste afficher.
            foreach (ModelFile file in FilesList)
            {

                var btn = Create_btn(file.Name, file.Date, file.Time, file.Weight.ToString()); //creation dynamique des boutton

                btn.Click += OnClickFichierDl; // Affectation de la fonction OnClickFichierDl lors du click

                ColumnFichier.Children.Add(btn);
            }
        }

        #endregion Methode














        public static string? Boutton_Name_File;  // R�cup�re globalement le nom du fichier clicker.

        public async void OnClickFichierDl(object? sender, RoutedEventArgs e) // Permet d'ouvrir la fenetre PopUp
        {
            if (sender is Button button)
            {
                Boutton_Name_File = button.Name;
                OptionPopUpWindows = new OptionPopUp();
                OptionPopUpWindows.Closed += (s, e) => refresh();  // Quand elle ce ferme on refresh la list des fichiers.
                await OptionPopUpWindows.ShowDialog(this);
            }
        }


































        private async void Add_File(IStorageFile file) // Permet de cr�� et d'ajouter un fichier � la BDD
        {


            DateTime date = DateTime.Now;

            string minute = Convert.ToString(date.Minute);
            if (date.Minute < 10) minute = "0" + date.Minute;
            
            string complet_date = $"{date.Day}/{date.Month}/{date.Year}";
            string complet_time = $"{date.Hour}:{minute}";


            // R�cup�re les infos importante du fichier (Nom, chemin, taille)

            string NameFile = file.Name.Replace(' ', '_');


            if (_libsglobal.CheckIfFileExist(NameFile))
            {

                FrmErrorPopUp PopUpWindows = new FrmErrorPopUp(); 

                await PopUpWindows.ShowDialog(this);

            }
            else
            {

                var FilePath = file.Path;
                string FileSize;
                using (var stream = await file.OpenReadAsync())
                    FileSize = Convert.ToString($"{stream.Length / 1024} Ko"); // Converti l'octet en Ko (et en string)
                





                #region A continuer











                // Permet de cr�� le boutton et l'ajouter � la liste des fichier.
                string ProjectFile = "";
                string Folder = "";
                if (Os() == "Windows")
                {
                    ProjectFile = Get_Current_Directory();
                    Folder = "Saved_File";
                }
                else if (Os() == "Linux")
                {
                    ProjectFile = "/usr/share/storg";
                    Folder = "Saved_File";

                }

                string Destination_Folder = Path.Combine(ProjectFile, Folder);
                string Destination_Path = Path.Combine(Destination_Folder, Name_file); // Cr�� les chemin pour enregistrer les fichiers



                // Permet de copier le fichier //

                await using (var InputStream = await file.OpenReadAsync())
                await using (var OutputStream = File.Create(Destination_Path))
                {
                    await InputStream.CopyToAsync(OutputStream);
                }

                var btn = Create_btn(Name_file, complet_date, complet_time, file_Size);

                btn.Click += OnClickFichierDl; // Ajoute de la fonction OnClickFichierDl quand le boutton est clicker

                ColumnFichier.Children.Add(btn); // Ajoute le boutton � la liste des fichiers.



                // Permet d'executer le script python pour ajouter un fichier � la BDD //

                exec_py = Path.Combine(Get_Current_Directory(), "Gestion_BDD");

                if (Os() == "Linux")
                {
                    exec_py = "/usr/share/storg/Gestion_BDD";

                }

                process.StartInfo.FileName = exec_py;
                process.StartInfo.Arguments = $"store_file {Name_file} {complet_date} {complet_time} {file_Size}";
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

                }



            }

            //////


           
            refresh(); // Refresh pour que le nouveau fichier soit en haut

        }












        private Button Create_btn(string Name_file, string complet_date, string complet_time, string file_Size) // Permet de cr�� un boutton (Utiliser par fonction Ajout file + refresh)
        {

            ///
            bool verif = false;

            Button btn = new Button // Cr�ation d'un nouveau boutton
            {
                ///// Param�tre du boutton /////
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 15, 0, 0),
                Height = 30,
                Background = new SolidColorBrush(Color.Parse("#42505f")),
                Name = Name_file,
                /////
            };
            btn.Classes.Add("File");

            if (verif == false)
            {
                complet_date = complet_date + " " + complet_time;
                verif = true;
            }

            List<string> Info_btn = [Name_file, complet_date, file_Size];
            Grid grid = new Grid();
            foreach (var i in Info_btn)
            {
                if (i == complet_date) // Affiche les parametre sur le boutton
                {
                    TextBlock txt_btn = new TextBlock
                    {
                        Text = i,
                        Margin = new Thickness(420, 0, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    grid.Children.Add(txt_btn);

                }
                else if (i == file_Size)
                {
                    TextBlock txt_btn = new TextBlock
                    {
                        Text = i,
                        Margin = new Thickness(580, 0, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    grid.Children.Add(txt_btn);
                }
                else
                {
                    TextBlock txt_btn = new TextBlock
                    {
                        Text = i,
                        Margin = new Thickness(10, 0, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    grid.Children.Add(txt_btn);
                }
                btn.Content = grid;
            }

            return btn;
        }













        public void Research(object? sender, KeyEventArgs e) // Faire une recherche de fichier
        {
            // if (e.Key == Key.Enter) // V�rifi si la touche entrer est presser
            // {
            //     string research_file_text = "";
            //     if (Search.Text != null) // Si c'est le cas on r�cup�re le texte �crit
            //     {
            //         research_file_text = Search.Text;
            //     }

            //     if (research_file_text != "")
            //     {

            //         List<string[]> File_List = new List<string[]>();

            //         string exec_py = Path.Combine(Get_Current_Directory(), "Gestion_BDD");

            //         if (Os() == "Linux")
            //         {
            //             exec_py = "/usr/share/storg/Gestion_BDD";

            //         }

            //         Process process = new Process();
            //         process.StartInfo.FileName = exec_py;
            //         process.StartInfo.Arguments = $"research_file {research_file_text}";
            //         process.StartInfo.RedirectStandardOutput = true;

            //         process.StartInfo.UseShellExecute = false;
            //         process.StartInfo.CreateNoWindow = true;


            //         process.Start();


            //         if (process == null)
            //         {
            //             return;
            //         }
            //         using (System.IO.StreamReader reader = process.StandardOutput)
            //         {

            //             string output = reader.ReadToEnd();

            //             // Le script python r�cup�re tout les fichier stocker dans la BDD pour les afficher //
            //             List<List<string>>? pythonList = JsonConvert.DeserializeObject<List<List<string>>>(output);
            //             if (pythonList == null)
            //             {
            //                 return;
            //             }

            //             foreach (var i in pythonList)
            //             {
            //                 List<string> Element = new List<string>();
            //                 foreach (var value in i)
            //                 {
            //                     Element.Add(value.ToString());
            //                 }
            //                 File_List.Add(Element.ToArray());
            //             }


            //         }



            //         File_List.Reverse(); // Inverse la liste pour avoir le fichier le plus r�cent en haut
            //         ColumnFichier.Children.Clear(); // Vide la liste afficher.
            //         foreach (var i in File_List)
            //         {

            //             // On recr�� tout les bouttons //

            //             var btn = Create_btn(i[0], i[1], i[2], i[3]);

            //             btn.Click += OnClickFichierDl; // Affectation de la fonction OnClickFichierDl lors du click

            //             ColumnFichier.Children.Add(btn);
            //         }


            //     }

            //     Focus();
            // }
            // if (e.Key == Key.Escape) // si escape est presser alors on quitte la zone de recherche
            // {
            //     Focus();
            //     refresh();
            // }
        }








        private void Focus(object sender, RoutedEventArgs e) // Permet de vider le texte de la TextBox quand on click dedans
        {

            if (Search.Text == "Rechercher des fichiers compress�s")
            {
                Search.Text = string.Empty;
            }

        }







        private void NoFocus(object sender, RoutedEventArgs e) // Permet de rem�tre le text de la Textbox quand on la quitte si elle est vide
        {

            if (string.IsNullOrWhiteSpace(Search.Text))
            {
                Search.Text = "Rechercher des fichiers compress�s";
            }

        }








        private void Lost_Focus(object sender, PointerPressedEventArgs e) // Si on click a coter alors on quitte la TextBox
        {

            if (!Search.IsPointerOver)
            {
                Focus();
            }


        }










        public static void Delete_File() // Permet de supprimer un fichier
        {

            string exec_py = Path.Combine(Get_Current_Directory(), "Gestion_BDD");

            if (Os() == "Linux")
            {
                exec_py = "/usr/share/storg/Gestion_BDD";

            }

            Process process = new Process();
            process.StartInfo.FileName = exec_py;
            process.StartInfo.Arguments = $"delete_file {Boutton_Name_File}";
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

            }

            OptionPopUpWindows.Close(); // Ferme automatiquement le PopUp quand le fichier est supprimer 
        }










        public static void Export_File()  // Exporter mes fichier compresser
        {

            string Download_Folder = Get_Download_Folder();

            string exec_py = Path.Combine(Get_Current_Directory(), "Gestion_BDD");

            if (Os() == "Linux")
            {
                exec_py = "/usr/share/storg/Gestion_BDD";

            }

            Process process = new Process();
            process.StartInfo.FileName = exec_py;
            process.StartInfo.Arguments = $"export_file {Boutton_Name_File} {Download_Folder}";
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

            }


            OptionPopUpWindows.Close(); // Ferme automatiquement le PopUp quand le fichier est supprimer 

        }












        public static void Download_File() // Re telecharger le fichier telle qu'il �tait
        {
            string Download_Folder = Get_Download_Folder();

            string exec_py = Path.Combine(Get_Current_Directory(), "Gestion_BDD");

            if (Os() == "Linux")
            {
                exec_py = "/usr/share/storg/Gestion_BDD";

            }

            Process process = new Process();
            process.StartInfo.FileName = exec_py;
            process.StartInfo.Arguments = $"download_file {Boutton_Name_File} {Download_Folder}";
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

            }


            OptionPopUpWindows.Close(); // Ferme automatiquement le PopUp quand le fichier est supprimer 
        }








        static string Get_Download_Folder() // R�cup�re le chemin du dossier telechargement selon l'OS
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) // V�rifie si l'utilisateur est sous windows ou MacOs
            {

                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"); // R�cup�ration du dossier Telechargement

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) // V�rifie si l'utilisateur est sous Linux
            {
                string? home = Environment.GetEnvironmentVariable("HOME");
                if (!string.IsNullOrEmpty(home))
                {
                    string? Download = Environment.GetEnvironmentVariable("XDG_DOWNLOAD_DIR");
                    return !string.IsNullOrEmpty(Download) ? Download : Path.Combine(home, "Downloads"); // R�cup�ration du dossier Telechargement

                }
            }
            throw new PlatformNotSupportedException("OS non support� !");


        }










        private async void OpenFileBorwser(object? sender, PointerPressedEventArgs e) // Permet d'ouvrir l'explorateur de fichier
        {

            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.StorageProvider != null)
            {
                var items = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions // R�cup�re le ou les fichiers selectionner
                {
                    Title = "S�lection votre fichier", // Titre de la fenetre
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

                            Add_File(file); // Ajouter les fichier s�lectionner � la BBD et � la liste
                        }
                    }
                }

            }

        }




    }



}










// public static class PythonManager // Class pour inisaliser Python DLL
// {

//     private static bool _IsOk = false;

//     public static void Initialize() // Inisialisation de Python DLL si _IsOk est false
//     {

//         if (!_IsOk)
//         {

//             //string Base = "";
//             string Current_File = AppContext.BaseDirectory;
//             // if (Current_File != null)
//             // {




//             //     var FolderInfo = new DirectoryInfo(Current_File); // Remonte dans les r�pertoir, �quivalmant � : ../../../
//             //     var ParentFolder = FolderInfo.Parent;

//             //     if (ParentFolder != null && ParentFolder.Parent != null && ParentFolder.Parent.Parent != null && ParentFolder.Parent.Parent.Parent != null)
//             //     {
//             //         Base = ParentFolder.Parent.Parent.Parent.FullName;
//             //     }
//             // }

//             string Os = MainWindow.Os();
//             if (Os == "Windows")
//             {


//                 string pythonPath = Path.Combine(Current_File, "python_311");
//                 Environment.SetEnvironmentVariable("PYTHONHOME", pythonPath);
//                 Environment.SetEnvironmentVariable("PYTHONPATH", Path.Combine(pythonPath, "Lib"));

//                 string pythonDllPath = Path.Combine(pythonPath, "python311.dll");
//                 if (!System.IO.File.Exists(pythonDllPath))
//                 {
//                     throw new Exception($"Python DLL introuvable : {pythonDllPath}");
//                 }
//                 Runtime.PythonDLL = pythonDllPath;
//                 PythonEngine.Initialize();
//                 _IsOk = true;
//             }
//             else if (Os == "Linux")
//             {

//                 //string pythonPath = Path.Combine(Current_File, "python");
//                 string pythonExec = ("/usr/lib/x86_64-linux-gnu/libpython3.11.so");
//                 // Path.Combine(pythonPath, "lib", "libpython3.11.so")
//                 //Environment.SetEnvironmentVariable("PYTHONHOME", pythonPath);
//                 //Environment.SetEnvironmentVariable("PYTHONPATH", pythonPath + "/lib/python3.11/site-packages");
//                 //Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", pythonPath + "/lib");

//                 Runtime.PythonDLL = pythonExec;
//                 //PythonEngine.PythonPath = pythonPath + "/lib/python3.11";
//                 //PythonEngine.PythonHome = pythonPath;
//                 PythonEngine.Initialize();
//                 _IsOk = true;

//             }


//         }


//     }


// }