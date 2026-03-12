using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using StorgCommon;
using StorgLibs;

namespace StorgUI;

public partial class ControlSettings : UserControl
{

    private LibsGlobal _libsGlobal = new LibsGlobal();
    private ModelCurrentOS _currentOS = new ModelCurrentOS();
    private static ModelSettings _settings = new ModelSettings();
    private string hidePassword = "***********************";

    public ControlSettings()
    {
        InitializeComponent();

        this.Loaded += OnLoadSettings;
        this.Loaded += OnLoadSetConnectionIcons;


        #region Event


        btnConnection.Click += TryConnection;
        btnDisconnection.Click += TryDisconnection;
        txtboxEmail.GotFocus += GotFocusEmail;
        txtboxPassword.GotFocus += GotFocusPassword;
        txtboxPassword.LostFocus += LostFocusPassword;
        switchTheme.IsCheckedChanged += UpdateSettingsTheme;
        hyperLinkUser.Click += OnClickWebSiteRedirect;



        #endregion Event
    }



    #region Trigger

    private async void TryConnection(object? sender, RoutedEventArgs e)
    {
        string email = txtboxEmail.Text ?? "";
        string password = txtboxPassword.Text != null && txtboxPassword.Text != hidePassword ? txtboxPassword.Text : _settings.password;

        Dictionary<int, bool> userInformation = await _libsGlobal.StartConnection(email, password);
        if (userInformation.First().Value)
        {
            txtConnectionStatus.Text = "Connecté";
            txtConnectionResult.Text = "Connexion réussie";
            _libsGlobal.UpdateSettingsCredentials(email, password, userInformation.First().Key);
            txtboxPassword.Text = hidePassword;
            check.IsVisible = true;
            cross.IsVisible = false;
            _settings.isConnected = true;
            _settings.login = email;
            _settings.password = password;
        }
        else
        {
            txtConnectionResult.Text = "login ou mot de passe incorrect";
        }
        txtConnectionResult.IsVisible = true;
    }

    private async void TryDisconnection(object? sender, RoutedEventArgs e)
    {
        _libsGlobal.DisconnectUser();
        check.IsVisible = false;
        cross.IsVisible = true;
    }

    private async void ToggleConnection(object? sender, RoutedEventArgs e)
    {
        Dictionary<int, bool> userInformation = await _libsGlobal.StartConnection(_settings.login, _settings.password);

        if (userInformation.First().Value)
        {
            _libsGlobal.UpdateSettingsCredentials(_settings.login, _settings.password, _settings.userId);
            check.IsVisible = true;
            cross.IsVisible = false;
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
        _settings.lightMode = !(bool)switchTheme.IsChecked!;
        _libsGlobal.UpdateSettingsThemeMode(_settings.lightMode);
        Application.Current!.RequestedThemeVariant = _settings.lightMode ? ThemeVariant.Light : ThemeVariant.Dark;

    }

    private void OnLoadSettings(object? sender, RoutedEventArgs e)
    {
        _settings = _libsGlobal.LoadSettings();
        this.SetLoadSettings();
    }

    private void OnLoadSetConnectionIcons(object? sender, RoutedEventArgs e)
    {
        if (_settings.isConnected)
        {
            check.IsVisible = true;
            cross.IsVisible = false;
        }
        else
        {
            check.IsVisible = false;
            cross.IsVisible = true;
        }
    }

    private void OnClickWebSiteRedirect(object? sender, RoutedEventArgs e)
    {
        this.Redirect();
    }


    #endregion Trigger


    #region Methode

    private void SetLoadSettings()
    {
        switchTheme.IsChecked = !_settings.lightMode;
        txtboxEmail.Text = _settings.login != "" ? _settings.login : "Email";
        txtboxPassword.Text = _settings.password != "" ? hidePassword : "Mot de passe";
    }

    private void Redirect()
    {
        string url = "https://storg.serveousercontent.com/";
        try
        {
            if (_libsGlobal.IsWindows())
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
            }
            else if (_libsGlobal.IsLinux())
            {
                Process.Start("xdg-open", url);
            }
            else if (_libsGlobal.IsOSX())
            {
                Process.Start("open", url);
            }
            else
            {
                throw new Exception();
            }
        }
        catch (System.Exception)
        {
            FrmErrorPopUp errorPopUp = new FrmErrorPopUp("Navigateur introuvable.\nVeuillez vous rendre sur : https://storg.serveousercontent.com/");
            errorPopUp.ShowDialog((Window)VisualRoot!);
        }
    }

    #endregion Methode

}