using StorgLibs;

namespace StorgTestUnitaire
{
    [TestClass]
    public sealed class TestUnit
    {

        private BDDHelper _bddhelper = new BDDHelper();
        private GestionFileHelper _gestionfilehelper = new GestionFileHelper();

        [TestMethod]
        public void TestBDDFileExist()
        {
            _bddhelper.IsBddExisting();
            string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string DirPath = Path.Combine(CurrentDirectory, ".data");
            string BDDFilePath = Path.Combine(DirPath, "BDD_Files_Info.db");
            Assert.IsTrue(File.Exists(BDDFilePath));
            Directory.Delete(DirPath);
        }

        [TestMethod]
        public void TestDeleteFileByPath()
        {
            string test1 = "user/appdata/data/file";
            string test2 = "C:/document/folder/file";
            string test3 = "user/space folder/file";

            Assert.AreEqual(_gestionfilehelper.GetParentPath(test1), "user/appdata/data");
            Assert.AreEqual(_gestionfilehelper.GetParentPath(test2), "C:/document/folder");
            Assert.AreEqual(_gestionfilehelper.GetParentPath(test3), "user/space folder");

        }

    }
}
