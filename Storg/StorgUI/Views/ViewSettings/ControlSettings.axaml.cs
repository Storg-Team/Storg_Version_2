using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StorgCommon;
using StorgLibs;

namespace StorgUI;

public partial class ControlSettings : UserControl
{

    private LibsGlobal _libsGlobal = new LibsGlobal();
    private static ModelSettings _settings = new ModelSettings();
    private string hidePassword = "***********************";

    public ControlSettings()
    {
        InitializeComponent();

        _settings = _libsGlobal.LoadSettings();
        this.SetLoadSettings();
        this.SetVisibility();

        #region Event


        btnConnection.Click += TryConnection;
        switchConnection.Click += ToggleConnection;
        txtboxEmail.GotFocus += GotFocusEmail;
        txtboxPassword.GotFocus += GotFocusPassword;
        txtboxPassword.LostFocus += LostFocusPassword;
        switchTheme.IsCheckedChanged += UpdateSettingsTheme;



        #endregion Event
    }



    #region Trigger

    private async void TryConnection(object? sender, RoutedEventArgs e)
    {
        string email = txtboxEmail.Text ?? "";
        string password = txtboxPassword.Text != null && txtboxPassword.Text != hidePassword ? txtboxPassword.Text : _settings.password;

        if (await _libsGlobal.StartConnection(email, password))
        {
            txtConnectionResult.Text = "Connexion réussi";
            _libsGlobal.UpdateSettingsCredentials(email, password);
            txtboxPassword.Text = hidePassword;
        }
        else
        {
            txtConnectionResult.Text = "login ou mot de passe incorrect";
        }
        txtConnectionResult.IsVisible = true;

    }

    private async void ToggleConnection(object? sender, RoutedEventArgs e)
    {
        _settings.canConnect = (bool)switchConnection.IsChecked!;
        this.SetVisibility();
        _libsGlobal.UpdateSettingsCanConnect(_settings.canConnect);
        if (!_settings.canConnect) _libsGlobal.UpdateSettingsCredentials(_settings.login, _settings.password, false);
        else
        {
            if (await _libsGlobal.StartConnection(_settings.login, _settings.password))
            {
                _libsGlobal.UpdateSettingsCredentials(_settings.login, _settings.password);
            }
        }
    }

    private void GotFocusEmail(object? sender, RoutedEventArgs e)
    {
        if (txtboxEmail.Text == "Email") txtboxEmail.Text = "";
    }

    private void GotFocusPassword(object? sender, RoutedEventArgs e)
    {
        if (txtboxPassword.Text == "Mot de passe" || txtboxPassword.Text == hidePassword)
        {
            txtboxPassword.Text = "";
            txtboxPassword.PasswordChar = '*';
        }
    }

    private void LostFocusPassword(object? sender, RoutedEventArgs e)
    {
        if (_settings.password != "" && txtboxPassword.Text == "")
        {
            txtboxPassword.Text = hidePassword;
        }
    }

    private void UpdateSettingsTheme(object? sender, RoutedEventArgs e)
    {
        _libsGlobal.UpdateSettingsThemeMode(!(bool)switchTheme.IsChecked!);
    }


    #endregion Trigger


    #region Methode

    private void SetVisibility()
    {
        txtboxEmail.IsEnabled = _settings.canConnect;
        txtboxPassword.IsEnabled = _settings.canConnect;
        btnConnection.IsEnabled = _settings.canConnect;
    }

    private void SetLoadSettings()
    {
        switchTheme.IsChecked = !_settings.lightMode;
        switchConnection.IsChecked = _settings.canConnect;
        txtboxEmail.Text = _settings.login != "" ? _settings.login : "Email";
        txtboxPassword.Text = _settings.password != "" ? hidePassword : "Mot de passe";
    }


    #endregion Methode

}