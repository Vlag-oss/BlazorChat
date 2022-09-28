using BlazorChat.Server.Models;
using BlazorChat.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorChat.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly ILogger<SettingsController> _logger;
    private readonly BlazorChatContext _context;

    public SettingsController(ILogger<SettingsController> logger, BlazorChatContext context)
    {
        this._logger = logger;
        this._context = context;
    }

    [HttpGet("updatetheme")]
    public async Task<User> UpdateTheme(string userId, string value)
    {
        var user = await FindUser(Convert.ToInt32(userId));
        user.DarkTheme = value == "True" ? 1 : 0;

        await _context.SaveChangesAsync();
        return await Task.FromResult(user);
    }

    [HttpGet("updatenotifications")]
    public async Task<User> UpdateNotifications(string userId, string value)
    {
        var user = await FindUser(Convert.ToInt32(userId));
        user.Notifications = value == "True" ? 1 : 0;

        await _context.SaveChangesAsync();
        return await Task.FromResult(user);
    }

    private async Task<User> FindUser(int userId)
        => await _context.Users.Where(user => user.UserId == userId).FirstOrDefaultAsync() ?? throw new ArgumentNullException();
}