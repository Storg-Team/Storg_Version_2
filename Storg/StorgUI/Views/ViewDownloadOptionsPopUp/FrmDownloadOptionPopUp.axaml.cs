using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StorgLibs;

namespace StorgUI
{
    public partial class OptionPopUp : Window
    {

        private LibsGlobal _libsglobal = new LibsGlobal();
        private string FileName;

        public OptionPopUp(string TransfetFileName)
        {
            InitializeComponent();
            FileName = TransfetFileName;

            var button = this.FindControl<Button>("Exporter");
            if (button != null)
            {
                button.Click += OnclickExp;
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

        #region Trigger
        private void OnclickDel(object? sender, RoutedEventArgs e)
        {
            Delete();
        }

        private void OnclickDl(object? sender, RoutedEventArgs e)
        {
            Download();
        }

        private void OnclickExp(object? sender, RoutedEventArgs e)
        {
            Export();
        }
        #endregion Trigger



        #region Methode

        private void Download() // Re telecharger le fichier telle qu'il etait
        {
            _libsglobal.DownloadFile(FileName);

            this.Close(); // Ferme automatiquement le PopUp quand le fichier est supprimer 
        }




        private void Delete() // Permet de supprimer un fichier
        {
            _libsglobal.DeleteFile(_libsglobal.GetStoredPath(FileName));
            
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

            this.Close(); // Ferme automatiquement le PopUp quand le fichier est supprimer 
        }










        private void Export()  // Exporter mes fichier compresser
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


        #endregion Methode




    }
}