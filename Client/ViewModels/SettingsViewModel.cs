using System.Net.Http.Json;
using BlazorChat.Shared.Models;
using Blazored.Toast.Services;

namespace BlazorChat.Client.ViewModels;

public class SettingsViewModel : ISettingsViewModel
{
    public long UserId { get; set; }
    public bool Notifications { get; set; }
    public bool DarkTheme { get; set; }

    private readonly HttpClient _httpClient;
    private readonly IToastService _toastService;

    public SettingsViewModel()
    {
        
    }

    public SettingsViewModel(HttpClient httpClient, IToastService toastService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _toastService = toastService;
    }

    public async Task GetProfile()
    {
        var user = await _httpClient.GetFromJsonAsync<User>($"profile/getprofile/{this.UserId}") ?? new User();
        LoadCurrentObject(user);
    }

    public async Task UpdateTheme()
    {
        User user = this;
        await _httpClient.PutAsJsonAsync($"settings/updatetheme/{this.UserId}", user);
        _toastService.ShowSuccess("Settings have been saved successfully");
    }

    public async Task UpdateNotifications()
    {
        User user = this;
        await _httpClient.PutAsJsonAsync($"settings/updatenotifications/{this.UserId}", user);
        _toastService.ShowSuccess("Settings have been saved successfully");
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
            DarkTheme = (user.DarkTheme == null || (long)user.DarkTheme == 0) ? false : true
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