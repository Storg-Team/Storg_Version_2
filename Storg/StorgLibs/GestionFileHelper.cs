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
        private IntPtr _libHandle;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] //function comes from C/C++ code, not C#
        private delegate bool CompressFileDelegate(string filepath, string savefilepath, string data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DecompressFileDelegate(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]
            string[] filelist, int size, string dlpath);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void GetFileDataDelegate(byte[] buffer, int bufferSize);

        private CompressFileDelegate _compressFile;
        private DecompressFileDelegate _decompressFile;
        private GetFileDataDelegate _getFileData;
        private bool _disposed = false;


        private static string _savedFolder = ConfigurationManager.AppSettings.Get("SavedFolder")!;
        private static string _currentExecDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private SystemHelper _systemhelper = new SystemHelper();
        private BDDHelper _bddhelper = new BDDHelper();


        #region Methode
        public string GetLibPath()
        {
            string libPath = "";

            if (_systemhelper.IsWindows())
            {
                libPath = Path.Combine(_currentExecDirectory, "Libs", "libs_filecompression.dll");
            }
            else if (_systemhelper.IsLinux())
            {
                libPath = Path.Combine(_currentExecDirectory, "Libs", "libs_filecompression.so");
            }
            else if (_systemhelper.IsOSX())
            {
                libPath = Path.Combine(_currentExecDirectory,"Libs", "libs_filecompression.dylib");
            }
            else
            {
                throw new PlatformNotSupportedException($"Platform '{RuntimeInformation.OSDescription}' is not supported.");
            }

            // Verify file exists
            if (!File.Exists(libPath))
            {
                throw new FileNotFoundException($"Native library not found at: {libPath}");
            }

            return libPath;
        }
        public GestionFileHelper()
        {
            try
            {
                string libPath = GetLibPath();

                _libHandle = NativeLibrary.Load(libPath);

                IntPtr compressFuncPointer = NativeLibrary.GetExport(_libHandle, "CompressFile");
                _compressFile = Marshal.GetDelegateForFunctionPointer<CompressFileDelegate>(compressFuncPointer);

                IntPtr decompressFuncPointer = NativeLibrary.GetExport(_libHandle, "DecompressFile");
                _decompressFile = Marshal.GetDelegateForFunctionPointer<DecompressFileDelegate>(decompressFuncPointer);

                IntPtr getFileDataFuncPointer = NativeLibrary.GetExport(_libHandle, "GetFileData");
                _getFileData = Marshal.GetDelegateForFunctionPointer<GetFileDataDelegate>(getFileDataFuncPointer);
            }

            catch (DllNotFoundException ex)
            {
                throw new Exception(
                    $"Failed to load native library. " +
                    $"Current architecture: {RuntimeInformation.ProcessArchitecture}. " +
                    $"Make sure the library is compiled for the correct architecture. " +
                    $"Original error: {ex.Message}", ex);
            }
        }
        public async Task<bool> StoreFile(string fileName, string filePath, string fileSize)
        {
            string Destination_Path = Path.Combine(_systemhelper.GetDestinationFolder(), fileName);

            Directory.CreateDirectory(Destination_Path);

            byte[] filedata = File.ReadAllBytes(filePath);
            string encodedBase64 = Convert.ToBase64String(filedata);

            try
            {
                if (_compressFile(filePath, Destination_Path, encodedBase64))
                {

                    if (_bddhelper.StoreFileToBDD(new ModelFile
                    {
                        Name = fileName,
                        Date = _systemhelper.GetDateTime().Date!,
                        Time = _systemhelper.GetDateTime().Time!,
                        Weight = fileSize,
                        StoredFolder = Destination_Path,
                    })) return true;
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
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

            int size = _decompressFile(filelist, filelist.Length, "");

            byte[] result = new byte[size];
            _getFileData(result, size);
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
