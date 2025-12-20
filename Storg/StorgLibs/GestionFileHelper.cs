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
using System.Text.Unicode;
using System.Buffers.Text;


namespace StorgLibs
{
    public class GestionFileHelper
    {

        #region DLL import
        private const string _libsName = "Libs/libs_filecompression.so";

        [DllImport(_libsName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CompressFile(string filepath, string savefilepath, string data);

        [DllImport(_libsName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int DecompressFile(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]
            string[] filelist, int size, string dlpath);

        [DllImport(_libsName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetFileData(byte[] buffer, int bufferSize);

        #endregion DLL import

        private ModelCurrentOS _currentOs = new ModelCurrentOS();
        private static string _savedFolder = ConfigurationManager.AppSettings.Get("SavedFolder")!;
        private static string _currentExecDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private SystemHelper _systemhelper = new SystemHelper();
        private BDDHelper _bddhelper = new BDDHelper();
        private string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);


        #region Methode
        public async Task<bool> StoreFile(string fileName, string filePath, string fileSize)
        {
            string destinationFolder = "";

            if (_systemhelper.GetCurrentOS() == _currentOs.Windows) // Cree les chemin pour enregistrer les fichiers
            {
                destinationFolder = Path.Combine(_currentExecDirectory, _savedFolder);
            }
            else if (_systemhelper.GetCurrentOS() == _currentOs.Linux)
            {
                destinationFolder = Path.Combine(Path.Combine(home, "storg"), ".data/SavedFolder");
            }

            string destinationPath = Path.Combine(destinationFolder, fileName);

            Directory.CreateDirectory(destinationPath);

            byte[] filedata = File.ReadAllBytes(filePath);
            string encodedBase64 = Convert.ToBase64String(filedata);

            if (CompressFile(filePath, destinationPath, encodedBase64))
            {

                if (_bddhelper.StoreFileToBDD(new ModelFile
                {
                    Name = fileName,
                    Date = _systemhelper.GetDateTime().Date!,
                    Time = _systemhelper.GetDateTime().Time!,
                    Weight = fileSize,
                    StoredFolder = destinationPath,
                })) return true;
                return false;
            }
            return false;
        }

        public async Task<bool> DownloadFile(string fileName)
        {
            string DownloadFolder = _systemhelper.GetDownloadFolder();

            if (!File.Exists(Path.Combine(DownloadFolder, fileName)))
            {
                
                File.WriteAllBytes(Path.Combine(DownloadFolder, fileName), this.FileToDecompress(fileName));

                DeleteFile(fileName);
                _bddhelper.DeleteFileInBDD(fileName);

                return true;
            }
            return false;
        }

        private byte[] FileToDecompress(string fileName)
        {
            string[] filelist = GetFileImageListe(fileName);

            int size = DecompressFile(filelist, filelist.Length, "");

            byte[] result = new byte[size];
            GetFileData(result, size);
            string encodedBase64 = Encoding.UTF8.GetString(result, 0, size);
            byte[] decodedBase64 = Convert.FromBase64String(encodedBase64);
            return decodedBase64;
        }

        public bool DeleteFile(string fileName)
        {
            string storedFilePath = _bddhelper.GetStoredPath(fileName);

            if (Directory.Exists(storedFilePath))
            {
                Directory.Delete(storedFilePath, recursive: true);
                _bddhelper.DeleteFileInBDD(fileName);
                if (Directory.Exists(storedFilePath))
                {
                    return false;
                }
            }
            return true;
        }

        public string GetParentPath(string storedFilePath, string fileName)
        {
            Regex regex = new Regex(@$"(.*?)(?={Regex.Escape($"{fileName}")}$)");
            return regex.Match(storedFilePath).Groups[1].Value;
        }

        public async Task<bool> ExportFile(string fileName)
        {
            string DownloadFolder = _systemhelper.GetDownloadFolder();
            string DownloadFileFolder = Path.Combine(DownloadFolder, "Dir_" + fileName);

            Directory.CreateDirectory(DownloadFileFolder);

            if (Directory.Exists(_bddhelper.GetStoredPath(fileName)))
            {
                string[] FilePathList = GetFileImageListe(fileName);

                for (int i = 0; i < FilePathList.Length; i++)
                {
                    File.Copy(FilePathList[i], Path.Combine(DownloadFileFolder, "img" + i + ".webp"));
                }
                return true;
            }

            return false;
        }

        public async Task ReplaceOnExportOrDownload(string fileName, bool isFile = true)
        {
            if (isFile)
            {
                string DownloadFilePath = Path.Combine(_systemhelper.GetDownloadFolder(), fileName);
                File.Delete(DownloadFilePath);
                await DownloadFile(fileName);
            }
            else
            {
                string DownloadFolderPath = Path.Combine(_systemhelper.GetDownloadFolder(), "Dir_" + fileName);
                Directory.Delete(DownloadFolderPath, recursive: true);
                await ExportFile(fileName);
            }
        }

        public bool CheckIfExistInDownloadFolder(string fileName, bool isFile = true)
        {
            if (isFile)
            {
                if (File.Exists(Path.Combine(_systemhelper.GetDownloadFolder(), fileName)))
                    return true;
            }
            else
            {
                if (Directory.Exists(Path.Combine(_systemhelper.GetDownloadFolder(), "Dir_" + fileName)))
                    return true;
            }

            return false;
        }

        private string[] GetFileImageListe(string FileName)
        {
            IList<KeyValuePair<int, string>> ImageListeKeyValue = new List<KeyValuePair<int, string>>();

            Regex regex = new Regex(@"/img(\d{0,}).webp");

            foreach (string imageName in Directory.GetFiles(_bddhelper.GetStoredPath(FileName)))
            {
                int imageNumber = Int16.Parse(regex.Match(imageName).Groups[1].Value);
                ImageListeKeyValue.Add(new KeyValuePair<int, string>(imageNumber, $"{imageName}"));
            }
            return ImageListeKeyValue.OrderBy(f => f.Key).Select(f => f.Value).ToArray();
        }


        #endregion Methode
    }
}
