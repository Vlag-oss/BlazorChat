using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace BlazorChat.Client.Handlers
{
    public class CustomAuthorizationHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorageService;

        public CustomAuthorizationHandler(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var jwtToken = await _localStorageService.GetItemAsync<string>("jwt_token");

            if (jwtToken != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
