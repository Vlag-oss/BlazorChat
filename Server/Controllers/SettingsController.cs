using BlazorChat.Server.Models;
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

    [HttpPut("updatetheme/{userId}")]
    public async Task<User> UpdateTheme(string userId, [FromBody] User user)
    {
        var userToUpdate = await FindUser(Convert.ToInt32(userId));
        userToUpdate.DarkTheme = user.DarkTheme;

        await _context.SaveChangesAsync();
        return await Task.FromResult(user);
    }

    [HttpPut("updatenotifications/{userId}")]
    public async Task<User> UpdateNotifications(string userId, [FromBody] User user)
    {
        var userToUpdate = await FindUser(Convert.ToInt32(userId));
        userToUpdate.Notifications = user.Notifications;

        await _context.SaveChangesAsync();
        return await Task.FromResult(user);
    }

    private async Task<User> FindUser(int userId)
        => await _context.Users.Where(user => user.UserId == userId).FirstOrDefaultAsync() ?? throw new ArgumentNullException();
}