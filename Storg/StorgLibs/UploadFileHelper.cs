using System;
using System.IO.Compression;
using System.Threading.Tasks;
using StorgCommon;

namespace StorgLibs.Libs;

public class UploadFileHelper
{

    private BDDHelper _bddHelper = new BDDHelper();
    private SystemHelper _systemHelper = new SystemHelper();
    private APIHelper _apiHelper = new APIHelper();

    public async Task<bool> UploadFile(IList<ModelDisplayFiles> listFile)
    {
        foreach (ModelDisplayFiles file in listFile)
        {
            string storedFilePath = _bddHelper.GetStoredPath(file.Name);
            string outputPath = Path.Combine(_systemHelper.GetWorkSpace(), file.Name + ".zip");

            ZipFile.CreateFromDirectory(storedFilePath, outputPath);

            await _apiHelper.UploadFileApi(outputPath, _bddHelper.LoadSettings().userId);

            File.Delete(outputPath);
        }
        return true;

    }
}
