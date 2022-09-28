using System.Net.Http.Json;
using BlazorChat.Shared.Models;

namespace BlazorChat.Client.ViewModels;

public class ContactsViewModel : IContactsViewModel
{
    public List<Contact> Contacts { get; set; }
    private readonly HttpClient _httpClient;

    public ContactsViewModel()
    {
        
    }

    public ContactsViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<Contact>> GetVisibleContacts(int startIndex, int count)
    {
        var users = await _httpClient.GetFromJsonAsync<List<User>>($"contacts/getvisiblecontacts?startIndex={startIndex}&count={count}") ?? new List<User>();
        LoadCurrentObject(users);
        return Contacts;
    }

    public async Task<int> GetContactsCount() => await _httpClient.GetFromJsonAsync<int>("contacts/getcontactscount");

    private void LoadCurrentObject(List<User> users)
    {
        this.Contacts = new List<Contact>();
        users.ForEach(user => Contacts.Add(user));
    }
}