using System.Net.Http.Json;
using BlazorChat.Shared.Models;

namespace BlazorChat.Client.ViewModels;

public class SettingsViewModel : ISettingsViewModel
{
    public bool Notifications { get; set; }
    public bool DarkTheme { get; set; }

    private HttpClient _httpClient;

    public SettingsViewModel()
    {
        
    }

    public SettingsViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task GetProfile()
    {
        User user = await _httpClient.GetFromJsonAsync<User>("user/getprofile/1") ?? new User();
        LoadCurrentObject(user);
    }

    public async Task Save()
    {
        await _httpClient.GetFromJsonAsync<User>($"user/updatetheme?userId={1}&value={this.DarkTheme.ToString()}");
        await _httpClient.GetFromJsonAsync<User>($"user/updatenotifications?userId={1}&value={this.Notifications.ToString()}");
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
            Notifications = user.Notifications is null or 0 ? false : true,
            DarkTheme = user.DarkTheme is null or 0 ? false : true
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