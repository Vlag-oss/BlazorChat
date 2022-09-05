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

    [HttpGet]
    public List<Contact> Get()
    {
        var users = _context.Users.ToList();

        var contacts = users
                        .Select(user => new Contact(user.UserId, user.FirstName, user.LastName))
                        .ToList();

        return contacts;
    }

    [HttpPut("updateprofile/{userId}")]
    public async Task<User> UpdateProfile(int userId, [FromBody] User user)
    {
        var userToUpdate = await _context.Users
                                            .Where(user => user.UserId == userId)
                                            .FirstOrDefaultAsync() ?? new User();

        userToUpdate.FirstName = user.FirstName;
        userToUpdate.LastName = user.LastName;
        userToUpdate.EmailAddress = user.EmailAddress;

        await _context.SaveChangesAsync();

        return await Task.FromResult(user);
    }


    [HttpGet("getprofile/{userId}")]
    public async Task<User> GetProfile(int userId)
        => await _context.Users
                            .Where(user => user.UserId == userId)
                            .FirstOrDefaultAsync() ?? new User();
}
