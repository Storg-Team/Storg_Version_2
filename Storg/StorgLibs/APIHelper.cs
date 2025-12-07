using System;
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

    public bool VerifConnection()
    {
        return false;
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

}
