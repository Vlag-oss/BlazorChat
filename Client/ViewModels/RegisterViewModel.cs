using BlazorChat.Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;

namespace BlazorChat.Client.ViewModels
{
    public class RegisterViewModel : IRegisterViewModel
    {
        private readonly HttpClient _httpClient;
        [Required(ErrorMessage = "The email address is required")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "The password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "The first name is required")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "The last name is required")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        public RegisterViewModel()
        {
            
        }

        public RegisterViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task RegisterUser()
        {
            await _httpClient.PostAsJsonAsync<User>("user/createaccount", this);
        }

        public static implicit operator RegisterViewModel(User user)
        {
            return new RegisterViewModel
            {
                EmailAddress = user.EmailAddress!,
                Password = user.Password!,
                FirstName = user.FirstName!,
                LastName = user.LastName!
            };
        }

        public static implicit operator User(RegisterViewModel registerViewModel)
        {
            return new User
            {
                EmailAddress = registerViewModel.EmailAddress,
                Password = registerViewModel.Password,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName
            };
        }
    }
}
