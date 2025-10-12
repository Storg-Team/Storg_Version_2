using StorgCommon;
using StorgLibs;

namespace StorgTestUnitaire
{
    [TestClass]
    public sealed class TestUnit
    {

        private BDDHelper _bddhelper = new BDDHelper();
        private GestionFileHelper _gestionfilehelper = new GestionFileHelper();
        private ModelCurrentOS _currentOs = new ModelCurrentOS();

        [TestMethod]
        public void TestBDDFileExist()
        {
            _bddhelper.IsBddExisting();
            string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string DirPath = Path.Combine(CurrentDirectory, ".data");
            string BDDFilePath = Path.Combine(DirPath, "BDD_Files_Info.db");
            Assert.IsTrue(File.Exists(BDDFilePath));
        }

        [TestMethod]
        public void TestDeleteFileByPath()
        {
            string test1 = "user/appdata/data/file";
            string test2 = "C:/document/folder/file";
            string test3 = "user/space folder/file";

            Assert.AreEqual(_gestionfilehelper.GetParentPath(test1, "file"), "user/appdata/data/");
            Assert.AreEqual(_gestionfilehelper.GetParentPath(test2, "file"), "C:/document/folder/");
            Assert.AreEqual(_gestionfilehelper.GetParentPath(test3, "file"), "user/space folder/");

        }

        [TestMethod]
        public void TestModelCurrentOS()
        {
            Assert.AreEqual(_currentOs.Windows, "Windows");
            Assert.AreEqual(_currentOs.Linux, "Linux");
            Assert.AreEqual(_currentOs.OSX, "MacOs");
        }

        [TestMethod]
        public void TestBDDStorage()
        {

            ModelFile testfile = new ModelFile()
            {
                Name = "test",
                Date = "123",
                Time = "456",
                Weight = "100",
                StoredFolder = "User/appdate/test"
            };

            _bddhelper.StoreFileToBDD(testfile);

            Assert.AreEqual(_bddhelper.LoadStoredFile()[0].Name, "test");
            Assert.AreEqual(_bddhelper.LoadStoredFile()[0].Date, "123");
            Assert.AreEqual(_bddhelper.LoadStoredFile()[0].Time, "456");
            Assert.AreEqual(_bddhelper.LoadStoredFile()[0].StoredFolder, "User/appdate/test");


            Assert.IsTrue(_bddhelper.CheckIfFileExist("test"));

            Assert.AreEqual(_bddhelper.GetStoredPath("test"), "User/appdate/test");

            Assert.AreEqual(_bddhelper.LoadStoredFile()[0].Name, "test");
            Assert.AreEqual(_bddhelper.LoadStoredFile()[0].Date, "123");
            Assert.AreEqual(_bddhelper.LoadStoredFile()[0].Time, "456");
            Assert.AreEqual(_bddhelper.LoadStoredFile()[0].StoredFolder, "User/appdate/test");


            _bddhelper.DeleteFileInBDD("test");
            Assert.IsFalse(_bddhelper.CheckIfFileExist("test"));

        }

    }
}
