using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using BlazorChat.Shared.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorChat.Client;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;

    public CustomAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorageService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var currentUser = await GetUserByJWTAsync();

        if (currentUser != null && currentUser.EmailAddress != null)
        {
            var claimEmailAddress = new Claim(ClaimTypes.Name, currentUser.EmailAddress);
            var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, Convert.ToString(currentUser.UserId));

            var claimIdentity = new ClaimsIdentity(new[] { claimEmailAddress, claimNameIdentifier }, "serverAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimIdentity);

            return new AuthenticationState(claimsPrincipal);
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public async Task<User?> GetUserByJWTAsync()
    {
        var jwtToken = await _localStorageService.GetItemAsStringAsync("jwt_token");
        if (jwtToken == null) return null;

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "User/getuserbyjwt");
        requestMessage.Content = new StringContent(jwtToken);
        requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.SendAsync(requestMessage);

        var returnedUser = await response.Content.ReadFromJsonAsync<User>();

        if(returnedUser != null) return await Task.FromResult(returnedUser);

        return null;
    }
}