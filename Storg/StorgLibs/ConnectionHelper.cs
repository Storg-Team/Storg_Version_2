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

        if (_settings.canConnect)
        {
            try
            {

                Dictionary<int, bool> userInformation = await _apiHelper.StartConnection(_settings.login, _settings.password);
                if (userInformation.First().Value)
                {
                    _bddHelper.UpdateSettingsCredentials(_settings.login, _settings.password, _settings.userId, true);
                    _settings.isConnected = true;
                }
                else
                {
                    this.DisconnectUser();
                }
            }
            catch (Exception)
            {
                this.DisconnectUser();
            }
        }
    }

    private void DisconnectUser()
    {
        _bddHelper.UpdateSettingsCredentials(_settings.login, _settings.password, _settings.userId, false);
        _settings.isConnected = false;
    }

}
