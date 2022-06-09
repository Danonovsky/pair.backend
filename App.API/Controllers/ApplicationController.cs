using App.API.DAL;
using App.API.DAL.Models;
using App.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _accessor;

    public ApplicationController(ApplicationDbContext db, IHttpContextAccessor accessor)
    {
        _db = db;
        _accessor = accessor;
    }

    [Authorize]
    public async Task<IActionResult> AddApplication(AddApplication request)
    {
        var user = _accessor.HttpContext.User;
        _db.Applications.Add(new Application
        {
            Status = Status.Waiting,
            Vehicle = request.Vehicle,
            DateAdded = DateTime.UtcNow,
            UserId = GetUserId()
        });
        await _db.SaveChangesAsync();
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AcceptApplication(Guid applicationId)
    {
        var application = await _db.Applications
            .FirstOrDefaultAsync(_ => _.Id == applicationId);
        if (application is null) return BadRequest("Invalid application id");
        application.Status = Status.Accepted;
        await _db.SaveChangesAsync();
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectApplication(Guid applicationId)
    {
        var application = await _db.Applications
            .FirstOrDefaultAsync(_ => _.Id == applicationId);
        if (application is null) return BadRequest("Invalid application id");
        application.Status = Status.Rejected;
        await _db.SaveChangesAsync();
        return Ok();
    }

    [Authorize]
    public async Task<IActionResult> ListUserApplications()
    {
        var user = _accessor.HttpContext.User;
        var applications = await _db.Applications
            .Where(_ => _.UserId == GetUserId())
            .Select(_ => new ApplicationListItem
            {
                Id = _.Id,
                Make = _.Vehicle.Make,
                Model = _.Vehicle.Model,
                Status = _.Status,
                DateAdded = _.DateAdded
            })
            .ToListAsync();
        return Ok(applications);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListAllApplications()
    {
        var applications = await _db.Applications
            .Select(_ => new ApplicationListItem
            {
                Id = _.Id,
                Make = _.Vehicle.Make,
                Model = _.Vehicle.Model,
                Status = _.Status,
                DateAdded = _.DateAdded
            })
            .ToListAsync();
        return Ok(applications);
    }
    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListAcceptedApplications()
    {
        var applications = await _db.Applications
            .Where(_ => _.Status == Status.Accepted)
            .Select(_ => new ApplicationListItem
            {
                Id = _.Id,
                Make = _.Vehicle.Make,
                Model = _.Vehicle.Model,
                Status = _.Status,
                DateAdded = _.DateAdded
            })
            .ToListAsync();
        return Ok(applications);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListWaitingApplications()
    {
        var applications = await _db.Applications
            .Where(_ => _.Status == Status.Waiting)
            .Select(_ => new ApplicationListItem
            {
                Id = _.Id,
                Make = _.Vehicle.Make,
                Model = _.Vehicle.Model,
                Status = _.Status,
                DateAdded = _.DateAdded
            })
            .ToListAsync();
        return Ok(applications);
    }

    private Guid GetUserId()
    {
        var user = _accessor.HttpContext.User;
        var id = Guid.Parse(user.Claims.FirstOrDefault(_ => _.Type == "id").Value);
        return id;
    }
}