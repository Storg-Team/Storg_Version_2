using StorgLibs;

namespace StorgTestUnitaire
{
    [TestClass]
    public sealed class Test1
    {

        BDDHelper bddhelper = new BDDHelper();

        [TestMethod]
        public void TestBDDFileExist()
        {
            bddhelper.IsBddExisting();
            string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string DirPath = Path.Combine(CurrentDirectory, ".data");
            string BDDFilePath = Path.Combine(DirPath, "BDD_Files_Info.db");
            Assert.IsTrue(File.Exists(BDDFilePath));
        }

        
    }
}
