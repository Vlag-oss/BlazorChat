using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorChat.Client;
using BlazorChat.Client.Handlers;
using BlazorChat.Client.Logging;
using BlazorChat.Client.ViewModels;
using Blazored.LocalStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services
    .AddHttpClient<IProfileViewModel, ProfileViewModel>
    ("ProfileViewModelClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CustomAuthorizationHandler>();

builder.Services
    .AddHttpClient<IContactsViewModel, ContactsViewModel>
    ("ContactsViewModelClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CustomAuthorizationHandler>();

builder.Services
    .AddHttpClient<ISettingsViewModel, SettingsViewModel>
    ("SettingsViewModelClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CustomAuthorizationHandler>();

builder.Services
    .AddHttpClient<ILoginViewModel, LoginViewModel>
    ("LoginViewModelClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services
    .AddHttpClient<IRegisterViewModel, RegisterViewModel>
    ("RegisterViewModelClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<CustomAuthorizationHandler>();

builder.Services.AddLogging(logging =>
{
    var httpClient = builder.Services.BuildServiceProvider().GetRequiredService<HttpClient>();
    var authenticationStateProvider = builder.Services.BuildServiceProvider().GetRequiredService<AuthenticationStateProvider>();

    logging.SetMinimumLevel(LogLevel.Error);
    //logging.ClearProviders();
    logging.AddProvider(new ApplicationLoggerProvider(httpClient, authenticationStateProvider));
});

builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
