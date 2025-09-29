using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using StorgCommon;


namespace StorgLibs
{
    public class GestionFileHelper
    {
        private LibsGlobal _libsglobal = new LibsGlobal();
        private ModelCurrentOS _currentOs = new ModelCurrentOS();
        private string _savedFolder = ConfigurationManager.AppSettings.Get("SavedFolder")!;
        private string _currentExecDirectory = AppDomain.CurrentDomain.BaseDirectory;


        public void StoreFile(string FileName, string FilePath, string FileSize)
        {

            string Destination_Folder = "";

            if (_libsglobal.GetCurrentOS() == _currentOs.Windows) // Créé les chemin pour enregistrer les fichiers
            {
                Destination_Folder = Path.Combine(_currentExecDirectory, _savedFolder);
            }
            else if (_libsglobal.GetCurrentOS() == _currentOs.Linux)
            {
                Destination_Folder = Path.Combine(ConfigurationManager.AppSettings.Get("LinuxStoragePath")!, _savedFolder);
            }

            string Destination_Path = Path.Combine(Destination_Folder, FileName);

            Directory.CreateDirectory(Destination_Path);

            // Permet de copier le fichier //
            string DestinationFilePath = Path.Combine(Destination_Path, FileName);
            File.Copy(FilePath, DestinationFilePath);

            _libsglobal.StoreFileToBDD(new ModelFile
            {
                Name = FileName,
                Date = _libsglobal.GetDateTime().Date!,
                Time = _libsglobal.GetDateTime().Date!,
                Weight = FileSize,
                StoredFolder = DestinationFilePath,
            });

        }

        public void DownloadFile(string FileName)
        {
            string DownloadFolder = _libsglobal.GetDownloadFolder();

            File.Copy(_libsglobal.GetStoredPath(FileName), Path.Combine(DownloadFolder, FileName));

            _libsglobal.DeleteFileInBDD(FileName);
        }

        public void DeleteFile(string StoredFilePath)
        {
           
            


        }


    }
}
