namespace BlazorChat.Client.ViewModels;

public interface ILoginViewModel
{
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    public Task<string> LoginUser();
}