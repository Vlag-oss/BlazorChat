using Microsoft.AspNetCore.Mvc;
using BlazorChat.Shared;
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
}
