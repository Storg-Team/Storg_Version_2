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
            _libsglobal.DeleteFile(FileName);
            
            this.Close(); // Ferme automatiquement le PopUp quand le fichier est supprimer 
        }

        private void Export()  // Exporter mes fichier compresser
        {
            _libsglobal.ExportFile(FileName);

            this.Close(); // Ferme automatiquement le PopUp quand le fichier est supprimer 

        }


        #endregion Methode


    }
}