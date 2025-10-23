using System.Threading.Tasks;
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

        private async void Download() // Re telecharger le fichier telle qu'il etait
        {
            if (!_libsglobal.DownloadFile(FileName))
            {
                FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Echec du téléchargement du ficher");
                        await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
            }
            this.Close(); // Ferme automatiquement le PopUp quand le fichier est supprimer 
        }

        private async void Delete() // Permet de supprimer un fichier
        {
            if (!_libsglobal.DeleteFile(FileName))
            {
                FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Echec de le suppression du ficher");
                        await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
            }
            this.Close(); // Ferme automatiquement le PopUp quand le fichier est supprimer 
        }

        private async void Export()  // Exporter mes fichier compresser
        {
            if (!_libsglobal.ExportFile(FileName))
            {
                FrmErrorPopUp PopUpWindows = new FrmErrorPopUp("Echec de l'export du ficher");
                        await PopUpWindows.ShowDialog((Window)this.VisualRoot!);
            }
            this.Close(); // Ferme automatiquement le PopUp quand le fichier est supprimer 
        }


        #endregion Methode


    }
}