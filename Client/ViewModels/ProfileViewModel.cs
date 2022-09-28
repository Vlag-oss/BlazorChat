using System.Net.Http.Json;
using BlazorChat.Shared.Models;

namespace BlazorChat.Client.ViewModels;

public class ProfileViewModel : IProfileViewModel
{
    public long UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public string? AboutMe { get; set; }
    public string? Message { get; set; }
    public string? ProfilePicDataUrl { get; set; }

    private readonly HttpClient _httpClient;

    public ProfileViewModel()
    {
        
    }

    public ProfileViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task UpdateProfile()
    {
        User user = this;
        await _httpClient.PutAsJsonAsync<User>($"profile/updateprofile/{UserId}", user);
        Message = "Profile updated successfully";
    }

    public async Task GetProfile()
    {
        var user = await _httpClient.GetFromJsonAsync<User>($"profile/getprofile/{UserId}") ?? new User();
        LoadCurrentObject(user);
        Message = "Profile loaded successfully";
    }

    private void LoadCurrentObject(ProfileViewModel profileViewModel)
    {
        this.FirstName = profileViewModel.FirstName;
        this.LastName = profileViewModel.LastName;
        this.EmailAddress = profileViewModel.EmailAddress;
        this.AboutMe = profileViewModel.AboutMe;
        this.ProfilePicDataUrl = profileViewModel.ProfilePicDataUrl;
    }

    public static implicit operator ProfileViewModel(User user)
    {
        return new ProfileViewModel
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddress = user.EmailAddress,
            AboutMe = user.AboutMe,
            ProfilePicDataUrl = user.ProfilePicDataUrl,
        };
    }

    public static implicit operator User(ProfileViewModel profileViewModel)
    {
        return new User
        {
            UserId = profileViewModel.UserId,
            FirstName = profileViewModel.FirstName,
            LastName = profileViewModel.LastName,
            EmailAddress = profileViewModel.EmailAddress,
            AboutMe = profileViewModel.AboutMe,
            ProfilePicDataUrl = profileViewModel.ProfilePicDataUrl
        };
    }
}