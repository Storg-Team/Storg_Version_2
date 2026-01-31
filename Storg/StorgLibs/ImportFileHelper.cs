using System;
using System.Configuration;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using StorgCommon;

namespace StorgLibs.Libs;

public class ImportFileHelper
{

    private APIHelper _apiHelper = new APIHelper();
    private SystemHelper _systemHelper = new SystemHelper();
    private ModelCurrentOS _currentOS = new ModelCurrentOS();
    private BDDHelper _bddHelper = new BDDHelper();
    private string _storedFilePath = "";
    private string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);



    public ImportFileHelper()
    {
        _storedFilePath = _systemHelper.GetDestinationFolder();
    }

    public async Task<bool> ImportFileFromApi(ModelDisplayFetchFile fileName)
    {
        
        string fileNameWithNoExtention = this.GetFileNameWithNoExtention(fileName.fileName);
        fileNameWithNoExtention = fileNameWithNoExtention == "" ? fileName.fileName : fileNameWithNoExtention;

        if (!_bddHelper.CheckIfFileExistInBDD(fileNameWithNoExtention))
        {
            byte[] dataFile = await _apiHelper.ImportFileApi(fileName.fileName);
            string outputPath = Path.Combine(_systemHelper.GetWorkSpace(), fileName.fileName);
            string outputFolder = Path.Combine(_storedFilePath, fileNameWithNoExtention);

            File.WriteAllBytes(outputPath, dataFile);

            Directory.CreateDirectory(outputFolder);
            try
            {
                ZipFile.ExtractToDirectory(outputPath, outputFolder);

                ModelFile file = new ModelFile()
                {
                    Name = fileNameWithNoExtention,
                    Date = _systemHelper.GetDateTime().Date!,
                    Time = _systemHelper.GetDateTime().Time!,
                    StoredFolder = outputFolder,
                    Weight = this.GetFileWeight(outputPath),
                };

                _bddHelper.StoreFileToBDD(file);
                File.Delete(outputPath);
            }
            catch (Exception)
            {
                Directory.Delete(outputFolder);
                File.Delete(outputPath);
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }

    private string GetFileWeight(string filePath)
    {
        using FileStream fs = File.OpenRead(filePath);
        return $"{fs.Length / 1024} Ko";
    }

    public string GetFileNameWithNoExtention(string fileName)
    {
        Regex regex = new Regex(@"(.{0,}).zip");

        return regex.Match(fileName).Groups[1].Value;
    }

}
