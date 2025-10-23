using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using StorgCommon;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Numerics;


namespace StorgLibs
{
    public class GestionFileHelper
    {

        #region DLL import
        private const string _libsName = "Libs/libs_filecompression.so";

        [DllImport(_libsName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CompressFile(string filepath, string savefilepath);

        [DllImport(_libsName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool DecompressFile(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]
            string[] filelist, int size, string dlpath);
        #endregion DLL import

        private ModelCurrentOS _currentOs = new ModelCurrentOS();
        private string _savedFolder = ConfigurationManager.AppSettings.Get("SavedFolder")!;
        private string _currentExecDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private SystemHelper _systemhelper = new SystemHelper();
        private BDDHelper _bddhelper = new BDDHelper();
        private string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);


        #region Methode
        public bool StoreFile(string FileName, string FilePath, string FileSize)
        {
            string Destination_Folder = "";

            if (_systemhelper.GetCurrentOS() == _currentOs.Windows) // Cree les chemin pour enregistrer les fichiers
            {
                Destination_Folder = Path.Combine(_currentExecDirectory, _savedFolder);
            }
            else if (_systemhelper.GetCurrentOS() == _currentOs.Linux)
            {
                Destination_Folder = Path.Combine(Path.Combine(home, "storg"), _savedFolder);
            }

            string Destination_Path = Path.Combine(Destination_Folder, FileName);

            Directory.CreateDirectory(Destination_Path);

            // Permet de copier le fichier //

            if (CompressFile(FilePath, Destination_Path))
            {

                _bddhelper.StoreFileToBDD(new ModelFile
                {
                    Name = FileName,
                    Date = _systemhelper.GetDateTime().Date!,
                    Time = _systemhelper.GetDateTime().Time!,
                    Weight = FileSize,
                    StoredFolder = Destination_Path,
                });
                //File.Copy(FilePath, DestinationFilePath);
                return true;
            }
            return false;
        }

        public bool DownloadFile(string FileName)
        {
            string DownloadFolder = _systemhelper.GetDownloadFolder();

            if (!File.Exists(Path.Combine(DownloadFolder, FileName)))
            {
                //File.Move(_bddhelper.GetStoredPath(FileName), Path.Combine(DownloadFolder, FileName));
                string[] Filelist = Directory.GetFiles(_bddhelper.GetStoredPath(FileName)).ToArray();

                if (DecompressFile(Filelist, Filelist.Length, Path.Combine(DownloadFolder, FileName)))
                {
                    DeleteFile(FileName);
                    _bddhelper.DeleteFileInBDD(FileName);
                }

                return true;
            }
            return false;
        }

        public bool DeleteFile(string FileName)
        {
            string StoredFilePath = _bddhelper.GetStoredPath(FileName);

            if (Directory.Exists(StoredFilePath))
            {
                Directory.Delete(StoredFilePath, recursive: true);
                _bddhelper.DeleteFileInBDD(FileName);
                if (Directory.Exists(StoredFilePath))
                {
                    return false;
                }
            }
            return true;
        }

        public string GetParentPath(string StoredFilePath, string FileName)
        {
            Regex regex = new Regex(@$"(.*?)(?={Regex.Escape($"{FileName}")}$)");
            return regex.Match(StoredFilePath).Groups[1].Value;
        }

        public bool ExportFile(string FileName)
        {
            string DownloadFolder = _systemhelper.GetDownloadFolder();
            string DownloadFileFolder = Path.Combine(DownloadFolder, "Dir_" + FileName);
            Directory.CreateDirectory(DownloadFileFolder);

            if (Directory.Exists(_bddhelper.GetStoredPath(FileName)))
            {
                string[] FilePathList = Directory.GetFiles(_bddhelper.GetStoredPath(FileName));

                for (int i = 0; i < FilePathList.Length; i++)
                {
                    File.Copy(FilePathList[i], Path.Combine(DownloadFileFolder, "img" + i + ".webp"));
                }
                return true;
            }
            return false;
        }
        #endregion Methode
    }
}
