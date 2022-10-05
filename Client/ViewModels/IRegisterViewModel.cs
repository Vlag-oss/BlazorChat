using System.Net;

namespace BlazorChat.Client.ViewModels;

public interface IRegisterViewModel
{
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Task RegisterUser();
}