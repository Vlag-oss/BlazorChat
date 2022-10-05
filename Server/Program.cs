using BlazorChat.Server.Hubs;
using BlazorChat.Server.Logging;
using BlazorChat.Server.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});

builder.Services.AddEntityFrameworkSqlite().AddDbContext<BlazorChatContext>();

builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/user/notauthorized";
})
.AddGoogle(googleOptions => {
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

var loggerFactory = app.Services.GetService<ILoggerFactory>();
var serviceProvider = ((IApplicationBuilder)app).ApplicationServices.CreateScope().ServiceProvider;
var appDbContext = serviceProvider.GetRequiredService<BlazorChatContext>();
var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

loggerFactory!.AddProvider(new ApplicationLoggerProvider(appDbContext, httpContextAccessor));

app.UseResponseCompression();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.MapFallbackToFile("index.html");

app.Run();
