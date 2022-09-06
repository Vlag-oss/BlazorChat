using System.Net.Http.Json;
using System.Security.Claims;
using BlazorChat.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorChat.Client;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    public CustomAuthenticationStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var currentUser = await _httpClient.GetFromJsonAsync<User>("User/getcurrentuser");

        if (currentUser?.EmailAddress != null)
        {
            var claim = new Claim(ClaimTypes.Name, currentUser.EmailAddress);
            var claimIdentity = new ClaimsIdentity(new[] { claim }, "serverAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimIdentity);

            return new AuthenticationState(claimsPrincipal);
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

    }
}