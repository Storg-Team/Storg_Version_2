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
using System.IO.Compression;


namespace StorgLibs
{
    public class GestionFileHelper
    {
        private IntPtr _libHandle;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] //function comes from C/C++ code, not C#
        private delegate bool CompressFileDelegate(string filepath, string savefilepath, string data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool DecompressFileDelegate(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]
            string[] filelist, int size, string filePath);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void GetFileDataDelegate(byte[] buffer, int bufferSize);

        private CompressFileDelegate _compressFile;
        private DecompressFileDelegate _decompressFile;
        private GetFileDataDelegate _getFileData;


        private static string _savedFolder = ConfigurationManager.AppSettings.Get("SavedFolder")!;
        private static string _currentExecDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private SystemHelper _systemhelper = new SystemHelper();
        private BDDHelper _bddhelper = new BDDHelper();


        #region Methode
        public string GetLibPath()
        {
            string libPath;

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
            string destinationPath = Path.Combine(_systemhelper.GetDestinationFolder(), fileName);

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


                if (_compressFile(tmpFilePath, destinationPath, ""))
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
                if (!File.Exists(Path.Combine(downloadFolder, fileName)) && !Directory.Exists(Path.Combine(downloadFolder, fileName)))
                {

                    string[] filelist = GetFileImageListe(_bddhelper.GetStoredPath(fileName));

                    if (_decompressFile(filelist, filelist.Length, tmpFilePath))
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
            string downloadFolder = _systemhelper.GetDownloadFolder();
            string downloadFile = Path.Combine(downloadFolder, fileName + ".zip");
            string savedFile = _bddhelper.GetStoredPath(fileName);

            if (Directory.Exists(savedFile))
            {
                try
                {
                    ZipFile.CreateFromDirectory(savedFile, downloadFile);
                }
                catch (System.Exception)
                {
                    return false;
                }
                return true;
            }

            return false;
        }

        public async Task ReplaceOnExportOrDownload(string fileName, bool isFile = true)
        {
            if (isFile)
            {
                string downloadFilePath = Path.Combine(_systemhelper.GetDownloadFolder(), fileName);
                File.Delete(downloadFilePath);
                await DownloadFile(fileName);
            }
            else
            {
                string downloadFolderPath = Path.Combine(_systemhelper.GetDownloadFolder(), fileName+".zip");
                Directory.Delete(downloadFolderPath, recursive: true);
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
                if (Directory.Exists(Path.Combine(_systemhelper.GetDownloadFolder(), fileName+".zip")))
                    return true;
            }

            return false;
        }

        private string[] GetFileImageListe(string filePath)
        {
            IList<KeyValuePair<int, string>> ImageListeKeyValue = new List<KeyValuePair<int, string>>();

            Regex regex = new Regex(@"/img(\d{0,}).webp");

            foreach (string imageName in Directory.GetFiles(filePath))
            {
                int imageNumber = Int16.Parse(regex.Match(imageName).Groups[1].Value);
                ImageListeKeyValue.Add(new KeyValuePair<int, string>(imageNumber, $"{imageName}"));
            }
            return ImageListeKeyValue.OrderBy(f => f.Key).Select(f => f.Value).ToArray();
        }

        public async Task<bool> LiveDecompression(string filePath)
        {
            string workSpace = _systemhelper.GetWorkSpace();
            string filePathUnZip = Path.Combine(workSpace, Path.GetFileNameWithoutExtension(filePath));
            string fileName = Path.GetFileName(filePathUnZip);
            string tmpFilePath = Path.Combine(_systemhelper.GetWorkSpace(), fileName + ".txt");

            try
            {
                if (File.Exists(Path.Combine(_systemhelper.GetDownloadFolder(), fileName))) return false;

                ZipFile.ExtractToDirectory(filePath, filePathUnZip);

                string[] imageList = this.GetFileImageListe(filePathUnZip);

                if (_decompressFile(imageList, imageList.Length, tmpFilePath))
                {
                    using FileStream fs = new FileStream(tmpFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 61440 * 1024);
                    using FileStream output = new FileStream(Path.Combine(_systemhelper.GetDownloadFolder(), fileName), FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 61440 * 1024);

                    using FromBase64Transform transform = new FromBase64Transform();
                    using CryptoStream crypto = new CryptoStream(fs, transform, CryptoStreamMode.Read);

                    crypto.CopyTo(output);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (File.Exists(tmpFilePath)) File.Delete(tmpFilePath);
                if (Directory.Exists(filePathUnZip)) Directory.Delete(filePathUnZip, recursive: true);
            }
        }


        #endregion Methode
    }
}
