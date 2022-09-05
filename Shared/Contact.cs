namespace BlazorChat.Shared;

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
}