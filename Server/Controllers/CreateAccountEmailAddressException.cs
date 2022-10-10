namespace BlazorChat.Server.Controllers;

public class CreateAccountEmailAddressException : Exception
{
    public CreateAccountEmailAddressException(string message) : base(message)
    {
        
    }
}