using Authentication.RefreshToken.Data;
using Authentication.RefreshToken.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Authentication.RefreshToken.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<User>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(user => user.UserName == request.UserName, cancellationToken))
        {
            return Conflict(new { message = "Username already exists." });
        }

        var user = new User
        {
            UserName = request.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}

public record CreateUserRequest(string UserName, string Password);
