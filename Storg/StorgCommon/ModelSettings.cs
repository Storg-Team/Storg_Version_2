using System;

namespace StorgCommon;

public class ModelSettings
{
    public bool lightMode {get; set;} = true;
    public bool canConnect {get; set;} = false;
    public string login {get; set;} = "";
    public string password {get; set;} = "";
    public bool isConnected {get; set;} = false;
}
