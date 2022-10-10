using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BlazorChat.Server.Models;
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
    public async Task<ActionResult<User>> LoginUser(User user)
    {
        user.Password = Utility.Encrypt(user.Password!);
        var loggedInUser = await _context.Users
                                            .Where(u => u.EmailAddress == user.EmailAddress && u.Password == user.Password)
                                            .FirstOrDefaultAsync();

        if (loggedInUser is not null)
        {
            var claimEmail = new Claim(ClaimTypes.Email, loggedInUser.EmailAddress!);
            var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, loggedInUser.UserId.ToString());

            var claimIdentity = new ClaimsIdentity(new[]{ claimEmail, claimNameIdentifier }, "serverAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
            
            await HttpContext.SignInAsync(claimsPrincipal, GetAuthenticationProperties());
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
    
    [HttpGet("GoogleSignIn")]
    public async Task GoogleSignIn()
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, GetAuthenticationProperties());
    }

    [HttpPost("createaccount")]
    public async Task<ActionResult> CreateAccount(User user)
    {
        var emailAddressExists = _context.Users.FirstOrDefault(u => u.EmailAddress == user.EmailAddress);

        if (emailAddressExists == null)
        {
            user.Password = Utility.Encrypt(user.Password!);
            user.Source = "APPL";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        return Ok();
    }

    private static AuthenticationProperties GetAuthenticationProperties()
    {
        return new AuthenticationProperties
        {
            ExpiresUtc = DateTime.Now.AddMinutes(10),
            RedirectUri = "/profile"
        };
    }

    [HttpGet("notauthorized")]
    public IActionResult NotAuthorized()
    {
        return Unauthorized();
    }
}