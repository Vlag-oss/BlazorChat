using System.Net.Http.Json;
using BlazorChat.Shared;
using BlazorChat.Shared.Models;

namespace BlazorChat.Client.ViewModels;

public class ContactsViewModel : IContactsViewModel
{
    public List<Contact> Contacts { get; set; }

    private HttpClient _httpClient;

    public ContactsViewModel()
    {
        
    }

    public ContactsViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    
    public async Task GetContacts()
    {
        List<User> users = await _httpClient.GetFromJsonAsync<List<User>>("user/getcontacts") ?? throw new ArgumentNullException();
        LoadCurrentObject(users);
    }

    private void LoadCurrentObject(List<User> users)
    {
        this.Contacts = new List<Contact>();
        users.ForEach(user => Contacts.Add(user));
    }
}