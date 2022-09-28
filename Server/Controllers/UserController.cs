using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BlazorChat.Server.Models;
using BlazorChat.Shared.Models;
using Microsoft.AspNetCore.Authentication.Google;

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

    [HttpPost("loginuser")]
    public async Task<ActionResult<User>> LoginUser([FromBody] User user)
    {
        user.Password = Utility.Encrypt(user.Password!);
        var loggedInUser = await _context.Users
                                            .Where(u => u.EmailAddress == user.EmailAddress && u.Password == user.Password)
                                            .FirstOrDefaultAsync();

        if (loggedInUser is not null)
        {
            var claim = new Claim(ClaimTypes.Email, loggedInUser.EmailAddress!);

            var claimIdentity = new ClaimsIdentity(new[]{ claim }, "serverAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimIdentity);

            await HttpContext.SignInAsync(claimsPrincipal);
        }

        return await Task.FromResult(loggedInUser) ?? throw new InvalidOperationException();
    }

    [HttpGet("getcurrentuser")]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        var currentUser = new User();

        if(User.Identity is { IsAuthenticated: true })
        {
            currentUser = await _context.Users.Where(u => u.EmailAddress == User.FindFirstValue(ClaimTypes.Email)).FirstOrDefaultAsync();

            if(currentUser is null)
            {
                currentUser = new User
                {
                    UserId = _context.Users.Max(user => user.UserId) + 1,
                    EmailAddress = User.FindFirstValue(ClaimTypes.Email),
                    Password = Utility.Encrypt(currentUser!.EmailAddress!),
                    Source = "EXTL"
                };

                _context.Users.Add(currentUser);
                await _context.SaveChangesAsync();
            }
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
        var userToUpdate = await FindUser(userId);

        userToUpdate.FirstName = user.FirstName;
        userToUpdate.LastName = user.LastName;
        userToUpdate.EmailAddress = user.EmailAddress;
        userToUpdate.AboutMe = user.AboutMe;
        userToUpdate.ProfilePicDataUrl = user.ProfilePicDataUrl;

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

    [HttpGet("GoogleSignIn")]
    public async Task GoogleSignIn()
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/profile" });
    }

    [HttpGet("getvisiblecontacts")]
    public async Task<List<User>> GetVisibleContacts(int startIndex, int count)
        => await _context.Users.Skip(startIndex).Take(count).ToListAsync();

    [HttpGet("getcontactscount")]
    public async Task<int> GetContactsCount()
        => await _context.Users.CountAsync();

    private async Task<User> FindUser(int userId)
        => await _context.Users.Where(user => user.UserId == userId).FirstOrDefaultAsync() ?? throw new ArgumentNullException();
}
