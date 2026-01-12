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
using System.Security.Cryptography;


namespace StorgLibs
{
    public class GestionFileHelper
    {

        #region DLL import
        private const string _libsName = "Libs/libs_filecompression.so";

        [DllImport(_libsName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CompressFile(string filepath, string savefilepath, string data);

        [DllImport(_libsName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool DecompressFile(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]
            string[] filelist, int size, string filePath);

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
            string tmpFilePath = Path.Combine(_systemhelper.GetWorkSpace(), fileName + ".txt");

            try
            {
                using FileStream fs = File.OpenRead(filePath);
                using FileStream output = new FileStream(tmpFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 61440 * 1024);

                using ToBase64Transform transform = new ToBase64Transform();
                using CryptoStream crypto = new CryptoStream(output, transform, CryptoStreamMode.Write);

                fs.CopyTo(crypto);
                crypto.FlushFinalBlock();


                if (CompressFile(tmpFilePath, destinationPath, ""))
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

            }
            catch (Exception)
            {
                if (Directory.Exists(destinationPath)) Directory.Delete(destinationPath, recursive: true);
            }
            finally
            {
                if (File.Exists(tmpFilePath)) File.Delete(tmpFilePath);
            }
            return false;

        }

        public async Task<bool> DownloadFile(string fileName)
        {
            string downloadFolder = _systemhelper.GetDownloadFolder();
            string tmpFilePath = Path.Combine(_systemhelper.GetWorkSpace(), fileName + ".txt");

            try
            {
                if (!File.Exists(Path.Combine(downloadFolder, fileName)))
                {

                    string[] filelist = GetFileImageListe(fileName);

                    if (DecompressFile(filelist, filelist.Length, tmpFilePath))
                    {
                        using FileStream fs = new FileStream(tmpFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 61440 * 1024);
                        using FileStream output = new FileStream(Path.Combine(downloadFolder, fileName), FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 61440 * 1024);

                        using FromBase64Transform transform = new FromBase64Transform();
                        using CryptoStream crypto = new CryptoStream(fs, transform, CryptoStreamMode.Read);

                        crypto.CopyTo(output);


                        DeleteFile(fileName);
                        _bddhelper.DeleteFileInBDD(fileName);
                    }


                    return true;
                }
            }
            finally
            {
                if (File.Exists(tmpFilePath)) File.Delete(tmpFilePath);
            }
            return false;
        }

        // private byte[] FileToDecompress(string fileName)
        // {
        //     string[] filelist = GetFileImageListe(fileName);

        //     int size = DecompressFile(filelist, filelist.Length, "");

        //     byte[] result = new byte[size];
        //     GetFileData(result, size);
        //     string encodedBase64 = Encoding.UTF8.GetString(result, 0, size);
        //     byte[] decodedBase64 = Convert.FromBase64String(encodedBase64);
        //     return decodedBase64;
        // }

        public bool DeleteFile(string fileName)
        {
            string storedFilePath = _bddhelper.GetStoredPath(fileName);

            if (Directory.Exists(storedFilePath))
            {
                Directory.Delete(storedFilePath, recursive: true);
                if (Directory.Exists(storedFilePath))
                {
                    return false;
                }
            }
            _bddhelper.DeleteFileInBDD(fileName);
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

        public async Task LiveDecompression(string filePath)
        {
            
        }


        #endregion Methode
    }
}
