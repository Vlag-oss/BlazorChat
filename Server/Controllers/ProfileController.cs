using BlazorChat.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorChat.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ILogger<ProfileController> _logger;
    private readonly BlazorChatContext _context;

    public ProfileController(ILogger<ProfileController> logger, BlazorChatContext context)
    {
        this._logger = logger;
        this._context = context;
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

    private async Task<User> FindUser(int userId)
        => await _context.Users.Where(user => user.UserId == userId).FirstOrDefaultAsync() ?? throw new ArgumentNullException();
}