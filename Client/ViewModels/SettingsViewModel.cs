using System.Net.Http.Json;
using BlazorChat.Shared.Models;

namespace BlazorChat.Client.ViewModels;

public class SettingsViewModel : ISettingsViewModel
{
    public bool Notifications { get; set; }
    public bool DarkTheme { get; set; }

    private readonly HttpClient _httpClient;

    public SettingsViewModel()
    {
        
    }

    public SettingsViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task GetProfile()
    {
        var user = await _httpClient.GetFromJsonAsync<User>("profile/getprofile/1") ?? new User();
        LoadCurrentObject(user);
    }

    public async Task Save()
    {
        await _httpClient.GetFromJsonAsync<User>($"settings/updatetheme?userId={1}&value={this.DarkTheme}");
        await _httpClient.GetFromJsonAsync<User>($"settings/updatenotifications?userId={1}&value={this.Notifications}");
    }

    private void LoadCurrentObject(SettingsViewModel settingsViewModel)
    {
        this.Notifications = settingsViewModel.Notifications;
        this.DarkTheme = settingsViewModel.DarkTheme;
    }

    public static implicit operator SettingsViewModel(User user)
    {
        return new SettingsViewModel
        {
            Notifications = user.Notifications is not (null or 0),
            DarkTheme = user.DarkTheme is not (null or 0)
        };
    }

    public static implicit operator User(SettingsViewModel settingsViewModel)
    {
        return new User
        {
            Notifications = settingsViewModel.Notifications ? 1 : 0,
            DarkTheme = settingsViewModel.DarkTheme ? 1 : 0
        };
    }

}