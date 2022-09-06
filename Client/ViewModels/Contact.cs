using BlazorChat.Shared.Models;

namespace BlazorChat.Client.ViewModels;

public class Contact
{
    public long ContactId {get;set;}
    public string FirstName {get;set;}
    public string LastName {get;set;}

    public Contact()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    public Contact(long contactId, string firstName, string lastName)
    {
        ContactId = contactId;
        FirstName = firstName;
        LastName = lastName;
    }

    public static implicit operator Contact(User user)
        => new Contact(user.UserId, user.FirstName, user.LastName);

    public static implicit operator User(Contact contact)
        => new User{ UserId = contact.ContactId, FirstName = contact.FirstName, LastName = contact.LastName };
}