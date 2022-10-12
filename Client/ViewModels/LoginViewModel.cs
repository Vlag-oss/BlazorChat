using System.Net.Http.Json;
using BlazorChat.Shared.Models;

namespace BlazorChat.Client.ViewModels;

public class LoginViewModel : ILoginViewModel
{
    public string EmailAddress { get; set; }
    public string Password { get; set; }

    private readonly HttpClient _httpClient;

    public LoginViewModel()
    {
        
    }

    public LoginViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task LoginUser()
    {
        await _httpClient.PostAsJsonAsync<User>("user/loginuser", this);
    }

    public async Task<AuthenticationResponse> AuthenticationJWT()
    {
        var httpResponseMessage = await _httpClient.PostAsJsonAsync("user/authenticatejwt", new AuthenticationRequest{  EmailAddress = this.EmailAddress, Password = this.Password});
        return await httpResponseMessage.Content.ReadFromJsonAsync<AuthenticationResponse>();
    }

    public static implicit operator LoginViewModel(User user)
    {
        return new LoginViewModel
        {
            EmailAddress = user.EmailAddress,
            Password = user.Password
        };
    }

    public static implicit operator User(LoginViewModel loginViewModel)
    {
        var user = new User
        {
            EmailAddress = loginViewModel.EmailAddress,
            Password = loginViewModel.Password
        };

        return user;
    }
}