using System;
using System.Linq.Expressions;
using System.Net.Cache;
using System.Threading.Tasks;
using StorgCommon;

namespace StorgLibs.Libs;

public class ConnectionHelper
{
    private BDDHelper _bddHelper = new BDDHelper();
    private static ModelSettings _settings = new ModelSettings();
    private static APIHelper _apiHelper = new APIHelper();


    public async Task VerifConnection()
    {
        _settings = _bddHelper.LoadSettings();

        if (_settings.isConnected && _settings.canConnect)
        {
            try
            {
                if (!await _apiHelper.StartConnection(_settings.login, _settings.password))
                {
                    _bddHelper.UpdateSettingsCredentials("", "", false);
                    _settings.isConnected = false;                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


    }

}
