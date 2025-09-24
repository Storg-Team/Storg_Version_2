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

        public ModelCurrentOS GetCurrentOS()
        {
            return _systemhelper.GetCurrentOS();
        }

        public bool CheckIfFileExist(string NameFIle)
        {
            return _bddhelper.CheckIfFileExist(NameFIle);
        }

    }
}
