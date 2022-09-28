namespace BlazorChat.Client.ViewModels;

public interface IContactsViewModel
{
    public Task<List<Contact>> GetVisibleContacts(int startIndex, int count);
    Task<int> GetContactsCount();
}