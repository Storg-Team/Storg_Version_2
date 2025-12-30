using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StorgCommon;


namespace StorgLibs.Libs;

public class APIHelper
{

    private static string _apiKey = "392023af96b3cf7b70720d0992fd3ec6749f4cb1ba5c2625a06fbc5e3d40b793";
    private ModelSettings _settings = new ModelSettings();
    private HttpClient _httpClient = new HttpClient();
    private BDDHelper _bddHelper = new BDDHelper();

    public APIHelper()
    {
        _httpClient.BaseAddress = new Uri("https://storgapi.serveousercontent.com");
    }


    public async Task<Dictionary<int, bool>> StartConnection(string login, string password)
    {
        this.UpdateSettings();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/connection?login={login}&mdp={password}&apiKey={_apiKey}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Dictionary<int, bool>>() ?? new Dictionary<int, bool> { { -1, false } };
            }
            return new Dictionary<int, bool> { { -1, false } };
        }
        catch (Exception)
        {
            return new Dictionary<int, bool> { { -1, false } };
        }

    }


    public async Task<bool> UploadFileApi(MultipartFormDataContent dataContent)
    {
        this.UpdateSettings();
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"/upload?userId={_settings.userId}&apiKey={_apiKey}", dataContent);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            return false;

        }
        catch (Exception)
        {
            return false;
        }


    }

    public async Task<IList<string>> GetFilesUploaded()
    {
        this.UpdateSettings();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(@$"/listuserfiles?userId={_settings.userId}&apiKey={_apiKey}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IList<string>>() ?? [];
            }

            return [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<byte[]> ImportFileApi(string fileName)
    {
        this.UpdateSettings();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(@$"/download?fileName={fileName}&userId={_settings.userId}&apiKey={_apiKey}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync() ?? [];
            }

            return [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<bool> DeleteFileApi(string fileName)
    {
        this.UpdateSettings();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(@$"/delete?fileName={fileName}&userId={_settings.userId}&apiKey={_apiKey}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
        }
        catch (Exception)
        {
            return false;
        }
        return false;
    }


    private void UpdateSettings()
    {
        _settings = _bddHelper.LoadSettings();
    }

}
