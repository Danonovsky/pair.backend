using Microsoft.EntityFrameworkCore;

namespace App.API.Models;

[Keyless]
public record UserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}