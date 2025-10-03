using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using StorgCommon;
using System.Runtime.CompilerServices;


namespace StorgLibs
{
    public class GestionFileHelper
    {
        private ModelCurrentOS _currentOs = new ModelCurrentOS();
        private string _savedFolder = ConfigurationManager.AppSettings.Get("SavedFolder")!;
        private string _currentExecDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private SystemHelper _systemhelper = new SystemHelper();
        private BDDHelper _bddhelper = new BDDHelper();
        


        public void StoreFile(string FileName, string FilePath, string FileSize)
        {

            string Destination_Folder = "";

            if (_systemhelper.GetCurrentOS() == _currentOs.Windows) // Créé les chemin pour enregistrer les fichiers
            {
                Destination_Folder = Path.Combine(_currentExecDirectory, _savedFolder);
            }
            else if (_systemhelper.GetCurrentOS() == _currentOs.Linux)
            {
                Destination_Folder = Path.Combine(ConfigurationManager.AppSettings.Get("LinuxStoragePath")!, _savedFolder);
            }

            string Destination_Path = Path.Combine(Destination_Folder, FileName);

            Directory.CreateDirectory(Destination_Path);

            // Permet de copier le fichier //
            string DestinationFilePath = Path.Combine(Destination_Path, FileName);

            _bddhelper.StoreFileToBDD(new ModelFile
            {
                Name = FileName,
                Date = _systemhelper.GetDateTime().Date!,
                Time = _systemhelper.GetDateTime().Time!,
                Weight = FileSize,
                StoredFolder = DestinationFilePath,
            });
            File.Copy(FilePath, DestinationFilePath);

        }

        public void DownloadFile(string FileName)
        {
            string DownloadFolder = _systemhelper.GetDownloadFolder();

            File.Move(_bddhelper.GetStoredPath(FileName), Path.Combine(DownloadFolder, FileName));

            _bddhelper.DeleteFileInBDD(FileName);
        }

        public void DeleteFile(string FileName)
        {
            string StoredFilePath = _bddhelper.GetStoredPath(FileName);

            Directory.Delete(GetParentPath(StoredFilePath));

            _bddhelper.DeleteFileInBDD(FileName);
        }

        public string GetParentPath(string StoredFilePath)
        {
            List<string> ParentPath = StoredFilePath.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            ParentPath.RemoveAt(ParentPath.Count - 1);
            return String.Join("/", ParentPath);
        }

        public void ExportFile(string FileName)
        {
            string DownloadFolder = _systemhelper.GetDownloadFolder();
            string DownloadFileFolder = Path.Combine(DownloadFolder, FileName);
            Directory.CreateDirectory(DownloadFileFolder);

            if (!File.Exists(Path.Combine(DownloadFileFolder, FileName)))
            {
                File.Copy(_bddhelper.GetStoredPath(FileName), Path.Combine(DownloadFileFolder, FileName));
            }

        }

    }
}
