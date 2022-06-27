﻿using Project.Shared.Abstractions.Models;

namespace Identity.API.Models;

public class UserWithJwtDto : UserDto
{
    public JwtDto JwtDto { get; set; }
}