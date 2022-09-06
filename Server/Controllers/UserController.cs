using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BlazorChat.Server.Models;
using BlazorChat.Shared.Models;

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

    [HttpPost("loginuser")]
    public async Task<ActionResult<User>> LoginUser([FromBody] User user)
    {
        User loggedInUser = await _context.Users
                                            .Where(u => u.EmailAddress == user.EmailAddress && u.Password == user.Password)
                                            .FirstOrDefaultAsync() ?? throw new ArgumentNullException();
        
        var claimEmailAddress = new Claim(ClaimTypes.Name, loggedInUser.EmailAddress);
        var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, Convert.ToString(loggedInUser.UserId));

        var claimIdentity = new ClaimsIdentity(new[]{ claimEmailAddress, claimNameIdentifier }, "serverAuth");
        var claimsPrincipal = new ClaimsPrincipal(claimIdentity);

        await HttpContext.SignInAsync(claimsPrincipal);

        return await Task.FromResult(loggedInUser);
    }

    [HttpGet("getcurrentuser")]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        User currentUser = new User();

        if(User.Identity is { IsAuthenticated: true })
        {
            currentUser.EmailAddress = User.FindFirstValue(ClaimTypes.Name);
            currentUser.UserId = Convert.ToInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        return await Task.FromResult(currentUser);
    }

    [HttpGet("logoutuser")]
    public async Task<ActionResult<string>> LogoutUser()
    {
        await HttpContext.SignOutAsync();
        return "Success";
    }

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
