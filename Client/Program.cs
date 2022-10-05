using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorChat.Client;
using BlazorChat.Client.Logging;
using BlazorChat.Client.ViewModels;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient<ILoginViewModel, LoginViewModel>("BlazorChatClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddHttpClient<IProfileViewModel, ProfileViewModel>("BlazorChatClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddHttpClient<ISettingsViewModel, SettingsViewModel>("BlazorChatClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddHttpClient<IContactsViewModel, ContactsViewModel>("BlazorChatClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddLogging(logging =>
{
    var httpClient = builder.Services.BuildServiceProvider().GetRequiredService<HttpClient>();
    var authenticationStateProvider = builder.Services.BuildServiceProvider().GetRequiredService<AuthenticationStateProvider>();

    logging.SetMinimumLevel(LogLevel.Error);
    logging.ClearProviders();
    logging.AddProvider(new ApplicationLoggerProvider(httpClient, authenticationStateProvider));
});

builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
