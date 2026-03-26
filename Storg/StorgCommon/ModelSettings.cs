using System;

namespace StorgCommon;

public class ModelSettings
{
    public int userId {get; set;} = 0;
    public bool lightMode {get; set;} = true;
    public string login {get; set;} = "";
    public string password {get; set;} = "";
    public bool isConnected {get; set;} = false;
    public bool stayConnected {get; set;} = false;
}
