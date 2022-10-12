namespace BlazorChat.Shared.Models;

public class AuthenticationRequest
{
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}