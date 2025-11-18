using System.Buffers.Text;
using System.Text;
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
        private SystemHelper _systemhelper = new SystemHelper();

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
                Name = "testbdd",
                Date = "123",
                Time = "456",
                Weight = "100",
                StoredFolder = "User/appdate/test"
            };

            _bddhelper.StoreFileToBDD(testfile);

            Assert.AreEqual(_bddhelper.ResearchFileByName("testbdd")[0].Name, "testbdd");
            Assert.AreEqual(_bddhelper.ResearchFileByName("testbdd")[0].Date, "123");
            Assert.AreEqual(_bddhelper.ResearchFileByName("testbdd")[0].Time, "456");
            Assert.AreEqual(_bddhelper.ResearchFileByName("testbdd")[0].StoredFolder, "User/appdate/test");


            Assert.IsTrue(_bddhelper.CheckIfFileExist("testbdd"));

            Assert.AreEqual(_bddhelper.GetStoredPath("testbdd"), "User/appdate/test");

            Assert.AreEqual(_bddhelper.ResearchFileByName("testbdd")[0].Name, "testbdd");
            Assert.AreEqual(_bddhelper.ResearchFileByName("testbdd")[0].Date, "123");
            Assert.AreEqual(_bddhelper.ResearchFileByName("testbdd")[0].Time, "456");
            Assert.AreEqual(_bddhelper.ResearchFileByName("testbdd")[0].StoredFolder, "User/appdate/test");


            _bddhelper.DeleteFileInBDD("testbdd");
            Assert.IsFalse(_bddhelper.CheckIfFileExist("testbdd"));

        }

        [TestMethod]
        public void TestFileDelete()
        {
            long LengthRef;
            using (FileStream fs = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testdelete.pdf")))
            {

                byte[] text = new UTF8Encoding(true).GetBytes("c2pkaGZranNoZGZrZGVjZGV6ZGZlZmVkZmZlZGZmZmZmZmZmZmZmZmZmZmZmZmZmZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmVlZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWVlZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZGRkZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWZmZmZmZmZmZmZmZmZmZmZmZmZmZmZmZWVlZWVlZWVlZWVlZWVlZWVlZWVlZWVl");
                fs.Write(text);
                LengthRef = fs.Length;
            }

            ModelFile testfile = new ModelFile()
            {
                Name = "testdelete.pdf",
                Date = "123",
                Time = "456",
                Weight = "100",
                StoredFolder = "User/appdate/test"
            };

            Assert.IsTrue(_gestionfilehelper.StoreFile(testfile.Name, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testdelete.pdf"), "100"));

            Assert.IsTrue(Directory.Exists(_bddhelper.GetStoredPath(testfile.Name)));

            Assert.IsTrue(_gestionfilehelper.ExportFile(testfile.Name));

            Assert.IsTrue(Directory.Exists(Path.Combine(_systemhelper.GetDownloadFolder(), "Dir_"+testfile.Name)));

            Assert.IsTrue(_gestionfilehelper.DownloadFile(testfile.Name));

            Assert.IsTrue(File.Exists(Path.Combine(_systemhelper.GetDownloadFolder(), testfile.Name)));

            // using (FileStream fs = File.Open(Path.Combine(_systemhelper.GetDownloadFolder(), testfile.Name), FileMode.Open))
            // {
            //     Assert.Equals(LengthRef, fs.Length);
            // }

            Assert.IsTrue(_gestionfilehelper.DeleteFile(testfile.Name));

            File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testdelete.pdf"));
        }

        [TestMethod]
        public void TestCreationFile()
        {
            // byte[] result = File.ReadAllBytes("/home/lucas/Bureau/testdoc.odt");
            // string encoded = "";
            // string[] test = File.ReadAllLines("/home/lucas/Bureau/result.txt");
            // foreach (string item in test)
            // {   
            //     encoded += item;
            // }


            // string encoded = Convert.ToBase64String(result);

            // byte[] decoded = Convert.FromBase64String(encoded);

            // File.WriteAllBytes("/home/lucas/Bureau/result.odt", decoded);

        }

    }
}
