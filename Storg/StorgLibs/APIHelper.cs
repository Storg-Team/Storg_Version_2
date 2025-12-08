using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace StorgLibs.Libs;

public class APIHelper
{

    private static string _apiKey = "392023af96b3cf7b70720d0992fd3ec6749f4cb1ba5c2625a06fbc5e3d40b793";
    private static HttpClient _httpClient = new HttpClient();

    public APIHelper()
    {
        _httpClient.BaseAddress = new Uri("http://localhost:5067");
    }

  
    public async Task<bool> StartConnection(string login, string password)
    {
        bool IsConnected = false;
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/connection?login={login}&mdp={password}&apiKey={_apiKey}");
            if (response.IsSuccessStatusCode)
            {
                IsConnected = await response.Content.ReadFromJsonAsync<bool>();
            }
            return IsConnected;
        }
        catch (Exception)
        {
            return false;
        }

    }


    public async Task<bool> UploadFile(string filePath)
    {
        try
        {
            using MultipartFormDataContent dataContent = new MultipartFormDataContent();

            using FileStream fileStream = File.OpenRead(filePath);
            StreamContent streamContent = new StreamContent(fileStream);
            ByteArrayContent fileContent = new ByteArrayContent(streamContent.ReadAsByteArrayAsync().Result);
            dataContent.Add(fileContent, "file", Path.GetFileName(filePath));


            HttpResponseMessage response = await _httpClient.PostAsync($"/upload?apiKey={_apiKey}", dataContent);

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

}
