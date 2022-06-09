using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.API.DAL;
using Identity.API.DAL.Models;
using Identity.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project.Shared.Abstractions.Models;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IdentityDbContext _db;

    public AccountController(IdentityDbContext db)
    {
        _db = db;
    }

    [HttpPost("sign-up", Name = "Sign Up")]
    public async Task<IActionResult> SignUp(SignUp request)
    {
        if (request is null) return BadRequest();
        _db.Users.Add(new User
        {
            Email = request.Email,
            Password = request.Password,
            Role = request.Role,
            FirstName = request.FirstName,
            LastName = request.LastName,
        });
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("sign-in",Name = "Sign In")]
    public async Task<IActionResult> SignIn(SignIn request)
    {
        if (request is null) return BadRequest("Invalid client request");
        
        var user = await _db.Users
            .FirstOrDefaultAsync(_ => _.Email == request.Email && _.Password == request.Password);
        if (user is default(User)) return Unauthorized();

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secretPAIRjwtPrivateKey"));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(
            issuer: "https://localhost:5010",
            audience: "https://localhost:5010",
            claims: new List<Claim>(),
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: signinCredentials
            );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return Ok(new JwtDto
        {
            Token = tokenString
        });
    }

    [HttpGet(Name="List All"), Authorize]
    public async Task<List<User>> ListAll()
    {
        return await _db.Users.ToListAsync();
    }
}