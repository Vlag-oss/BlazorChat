using BlazorChat.Server.Models;
using BlazorChat.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorChat.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ContactsController : ControllerBase
{
    private readonly ILogger<ContactsController> _logger;
    private readonly BlazorChatContext _context;

    public ContactsController(ILogger<ContactsController> logger, BlazorChatContext context)
    {
        this._logger = logger;
        this._context = context;
    }

    [HttpGet("getvisiblecontacts")]
    public async Task<List<User>> GetVisibleContacts(int startIndex, int count)
        => await _context.Users.Skip(startIndex).Take(count).ToListAsync();

    [HttpGet("getcontactscount")]
    public async Task<int> GetContactsCount()
        => await _context.Users.CountAsync();
}