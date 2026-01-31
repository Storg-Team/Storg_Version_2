using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.InteropServices;
using StorgCommon;

namespace StorgLibs
{
    public class SystemHelper
    {
        public bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public bool IsOSX()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        public string GetDestinationFolder()
        {
            string destinationFolder = "";

            if (IsWindows()) // Cree les chemin pour enregistrer les fichiers
            {
                destinationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings.Get("SavedFolder")!);
            }
            else if (IsLinux() || IsOSX())
            {
                destinationFolder = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "storg"), ".data/SavedFolder");
            }
            return destinationFolder;
        }

        public ModelTime GetDateTime()
        {
            DateTime date = DateTime.Now;

            string minute = Convert.ToString(date.Minute);
            if (date.Minute < 10) minute = "0" + date.Minute;

            return new ModelTime() { Date = $"{date.Day}/{date.Month}/{date.Year}", Time = $"{date.Hour}:{minute}" };
        }

        public string GetDownloadFolder() // Récupère le chemin du dossier telechargement selon l'OS
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) // Vérifie si l'utilisateur est sous windows ou MacOs
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"); // Récupération du dossier Telechargement
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) // Vérifie si l'utilisateur est sous Linux
            {
                string? home = Environment.GetEnvironmentVariable("HOME");
                if (!string.IsNullOrEmpty(home))
                {
                    string? Download = Environment.GetEnvironmentVariable("XDG_DOWNLOAD_DIR");
                    return !string.IsNullOrEmpty(Download) ? Download : Path.Combine(home, "Downloads"); // Récupèration du dossier Telechargement

                }
            }
            throw new PlatformNotSupportedException("OS non supporté !");
        }

        public string GetWorkSpace()
        {
            if (this.IsWindows()) return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".data");
            else if (this.IsOSX() || this.IsLinux()) return Path.Combine(Environment.GetEnvironmentVariable("HOME")!, "storg");
            throw new PlatformNotSupportedException("OS non supporté !");
        }
    }
}
