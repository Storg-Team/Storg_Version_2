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
        try
        {
            if (!_settings.stayConnected)
            {
                await this.DisconnectUser();
                return;
            }
            Dictionary<int, bool> userInformation = await _apiHelper.StartConnection(_settings.login, _settings.password);
            if (userInformation.First().Value)
            {
                _bddHelper.UpdateSettingsCredentials(_settings.login, _settings.password, _settings.userId, true);
                _settings.isConnected = true;
            }
            else
            {
                await this.DisconnectUser();
            }
        }
        catch (Exception)
        {
            await this.DisconnectUser();
        }
    }

    public async Task DisconnectUser()
    {
        if (!_settings.stayConnected)
        {
            _bddHelper.UpdateSettingsCredentials("", "", _settings.userId, false);
        }
        else
        {
            _bddHelper.UpdateSettingsCredentials("", _settings.password, _settings.userId, false);
        }

        _settings.isConnected = false;
    }

}
