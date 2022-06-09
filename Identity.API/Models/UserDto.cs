using Project.Shared.Abstractions.Models;

namespace Identity.API.Models;

public class UserDto
{
    public JwtDto JwtDto { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}