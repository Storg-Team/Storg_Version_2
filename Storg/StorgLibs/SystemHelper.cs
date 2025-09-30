using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using StorgCommon;

namespace StorgLibs
{
    public class SystemHelper
    {

        private ModelCurrentOS _currentOS = new ModelCurrentOS();

        public string GetCurrentOS()
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return _currentOS.Windows;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return _currentOS.Linux;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return _currentOS.OSX;
            throw new PlatformNotSupportedException("OS non supporté !");

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
    }
}
