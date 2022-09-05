namespace BlazorChat.Shared;

public class Contact
{
    public int ContactId {get;set;}
    public string FirstName {get;set;}
    public string LastName {get;set;}

    public Contact()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    public Contact(int contactId, string firstName, string lastName)
    {
        ContactId = contactId;
        FirstName = firstName;
        LastName = lastName;
    }
}