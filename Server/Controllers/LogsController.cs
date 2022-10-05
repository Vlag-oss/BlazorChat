using BlazorChat.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorChat.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class LogsController : ControllerBase
{
    private readonly BlazorChatContext _context;

    public LogsController(BlazorChatContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Log>>> GetLogs() 
        => await _context.Logs.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Log>> GetLog(long id)
    {
        var log = await _context.Logs.FindAsync(id);

        if (log == null)
            return NotFound();

        return log;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutLog(long id, Log log)
    {
        if (id != log.LogId)
            return BadRequest();

        _context.Entry(log).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LogExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Log>> PostLog(Log log)
    {
        _context.Logs.Add(log);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetLog", new { id = log.LogId }, log);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLog(long id)
    {
        var log = await _context.Logs.FindAsync(id);
        if (log == null)
            return NotFound();

        _context.Logs.Remove(log);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool LogExists(long id)
    {
        return _context.Logs.Any(e => e.LogId == id);
    }
}