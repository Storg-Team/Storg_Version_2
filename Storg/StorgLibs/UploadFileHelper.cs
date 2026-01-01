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

    public async Task<bool> UploadFileFromApi(ModelDisplayFiles file)
    {
        string storedFilePath = _bddHelper.GetStoredPath(file.Name);
        string outputPath = Path.Combine(_systemHelper.GetWorkSpace(), file.Name + ".zip");

        if (!File.Exists(outputPath)) ZipFile.CreateFromDirectory(storedFilePath, outputPath);

        MultipartFormDataContent dataContent = new MultipartFormDataContent();

        using (FileStream fileStream = File.OpenRead(outputPath))
        {
            StreamContent streamContent = new StreamContent(fileStream);
            ByteArrayContent fileContent = new ByteArrayContent(streamContent.ReadAsByteArrayAsync().Result);
            dataContent.Add(fileContent, "file", Path.GetFileName(outputPath));
        }

        bool uploadStatus = await _apiHelper.UploadFileApi(dataContent);

        if (File.Exists(outputPath)) File.Delete(outputPath);

        return uploadStatus;

    }
}
