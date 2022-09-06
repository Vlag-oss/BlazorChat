using Microsoft.AspNetCore.Mvc;
using BlazorChat.Shared;
using Microsoft.EntityFrameworkCore;
using BlazorChat.Server.Models;

namespace BlazorChat.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly BlazorChatContext _context;

    public UserController(ILogger<UserController> logger, BlazorChatContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("getcontacts")]
    public List<User> GetContacts() => _context.Users.ToList();

    [HttpPut("updateprofile/{userId}")]
    public async Task<User> UpdateProfile(int userId, [FromBody] User user)
    {
        User userToUpdate = await FindUser(userId);

        userToUpdate.FirstName = user.FirstName;
        userToUpdate.LastName = user.LastName;
        userToUpdate.EmailAddress = user.EmailAddress;

        await _context.SaveChangesAsync();

        return await Task.FromResult(user);
    }


    [HttpGet("getprofile/{userId}")]
    public async Task<User> GetProfile(int userId)
        => await FindUser(userId);

    [HttpGet("updatetheme")]
    public async Task<User> UpdateTheme(string userId, string value)
    {
        User user = await FindUser(Convert.ToInt32(userId));
        user.DarkTheme = value == "True" ? 1 : 0;

        await _context.SaveChangesAsync();
        return await Task.FromResult(user);
    }

    [HttpGet("updatenotifications")]
    public async Task<User> UpdateNotifications(string userId, string value)
    {
        User user = await FindUser(Convert.ToInt32(userId));
        user.Notifications = value == "True" ? 1 : 0;

        await _context.SaveChangesAsync();
        return await Task.FromResult(user);
    }

    private async Task<User> FindUser(int userId)
        => await _context.Users.Where(user => user.UserId == userId).FirstOrDefaultAsync() ?? throw new ArgumentNullException();
}
