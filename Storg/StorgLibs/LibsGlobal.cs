using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using StorgCommon;
using StorgLibs.Libs;

namespace StorgLibs
{
    public class LibsGlobal
    {
        private SystemHelper _systemhelper = new SystemHelper();
        private BDDHelper _bddhelper = new BDDHelper();
        private GestionFileHelper _gestionfilehelper = new GestionFileHelper();
        private static APIHelper _apiHelper = new APIHelper();
        private ConnectionHelper _connectionHelper = new ConnectionHelper();
        private UploadFileHelper _uploadFileHelper = new UploadFileHelper();


        public void IsBddExisting()
        {
            _bddhelper.IsBddExisting();
        }

        public IList<ModelFile> LoadStoredFile()
        {
            return _bddhelper.LoadStoredFile();
        }

        public string GetCurrentOS()
        {
            return _systemhelper.GetCurrentOS();
        }

        public bool CheckIfFileExistInBDD(string NameFIle)
        {
            return _bddhelper.CheckIfFileExistInBDD(NameFIle);
        }

        public ModelTime GetDateTime()
        {
            return _systemhelper.GetDateTime();
        }

        public async Task<bool> StoreFile(string FileName, string FilePath, string FileSize)
        {
            return await _gestionfilehelper.StoreFile(FileName, FilePath, FileSize);
        }

        public bool StoreFileToBDD(ModelFile file)
        {
            return _bddhelper.StoreFileToBDD(file);
        }

        public string GetDownloadFolder()
        {
            return _systemhelper.GetDownloadFolder();
        }

        public async Task<bool> DownloadFile(string FileName)
        {
            return await _gestionfilehelper.DownloadFile(FileName);
        }

        public string GetStoredPath(string FileName)
        {
            return _bddhelper.GetStoredPath(FileName);
        }

        public void DeleteFileInBDD(string FileName)
        {
            _bddhelper.DeleteFileInBDD(FileName);
        }

        public bool DeleteFile(string StoredFilePath)
        {
            return _gestionfilehelper.DeleteFile(StoredFilePath);
        }

        public async Task<bool> ExportFile(string FileName)
        {
            return await _gestionfilehelper.ExportFile(FileName);
        }

        public IList<ModelFile> ResearchFileByName(string ResearchText)
        {
            return _bddhelper.ResearchFileByName(ResearchText);
        }

        public async Task ReplaceOnExportOrDownload(string fileName, bool isFile = true)
        {
            await _gestionfilehelper.ReplaceOnExportOrDownload(fileName, isFile);
        }

        public bool CheckIfExistInDownloadFolder(string fileName, bool isFile = true)
        {
            return _gestionfilehelper.CheckIfExistInDownloadFolder(fileName, isFile);
        }

        public async Task<Dictionary<int, bool>> StartConnection(string login, string password)
        {
            return await _apiHelper.StartConnection(login, password);
        }

        public bool UpdateSettingsThemeMode(bool lightMode)
        {
            return _bddhelper.UpdateSettingsThemeMode(lightMode);
        }

        public bool UpdateSettingsCanConnect(bool canConnect)
        {
            return _bddhelper.UpdateSettingsCanConnect(canConnect);
        }

        public bool UpdateSettingsCredentials(string login, string password, int userId, bool isConnected = true)
        {
            return _bddhelper.UpdateSettingsCredentials(login, password, userId, isConnected);
        }

        public ModelSettings LoadSettings()
        {
            return _bddhelper.LoadSettings();
        }

        public async void VerifConnection()
        {
            await _connectionHelper.VerifConnection();
        }

        public async Task<IList<string>> GetFilesUploaded(int userId)
        {
            return await _apiHelper.GetFilesUploaded(userId);
        }

        public async Task<bool> UploadFile(IList<ModelDisplayFiles> listFile)
        {
            return await _uploadFileHelper.UploadFile(listFile);
        }
    }
}
