namespace BlazorChat.Client.ViewModels;

public interface ISettingsViewModel
{
    public bool Notifications { get; set; }
    public bool DarkTheme { get; set; }

    public Task GetProfile();
    public Task Save();
}