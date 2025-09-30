using System.Runtime.CompilerServices;
using StorgCommon;

namespace StorgLibs
{
    public class LibsGlobal
    {
        private SystemHelper _systemhelper = new SystemHelper();
        private BDDHelper _bddhelper = new BDDHelper();
        private GestionFileHelper _gestionfilehelper = new GestionFileHelper();

        public IList<ModelFile> LoadStoredFile()
        {
            return _bddhelper.LoadStoredFile();
        }

        public string GetCurrentOS()
        {
            return _systemhelper.GetCurrentOS();
        }

        public bool CheckIfFileExist(string NameFIle)
        {
            return _bddhelper.CheckIfFileExist(NameFIle);
        }

        public ModelTime GetDateTime()
        {
            return _systemhelper.GetDateTime();
        }

        public void StoreFile(string FileName, string FilePath, string FileSize)
        {
            _gestionfilehelper.StoreFile(FileName, FilePath, FileSize);
        }

        public void StoreFileToBDD(ModelFile file)
        {
            _bddhelper.StoreFileToBDD(file);
        }

        public string GetDownloadFolder()
        {
            return _systemhelper.GetDownloadFolder();
        }

        public void DownloadFile(string FileName)
        {
            _gestionfilehelper.DownloadFile(FileName);
        }

        public string GetStoredPath(string FileName)
        {
            return _bddhelper.GetStoredPath(FileName);
        }

        public void DeleteFileInBDD(string FileName)
        {
            _bddhelper.DeleteFileInBDD(FileName);
        }

        public void DeleteFile(string StoredFilePath)
        {
            _gestionfilehelper.DeleteFile(StoredFilePath);
        }

        public void ExportFile(string FileName)
        {
            _gestionfilehelper.ExportFile(FileName);
        }

        public IList<ModelFile> ResearchFileByName(string ResearchText)
        {
            return _bddhelper.ResearchFileByName(ResearchText);
        }

    }
}
