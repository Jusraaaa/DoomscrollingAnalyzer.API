using System.Security.Claims;
using DoomscrollingAnalyzer.API.Data;
using DoomscrollingAnalyzer.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoomscrollingAnalyzer.API.Controllers;

[ApiController]
[Authorize]
[Route("api/dev")]
public class DevController(AppDbContext context, IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost("seed-demo-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SeedDemoData()
    {
        if (!environment.IsDevelopment())
        {
            return Forbid();
        }

        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var now = DateTime.UtcNow;
        var entries = new List<ScreenTimeEntry>
        {
            CreateEntry(userId.Value, "TikTok", 2.4, now.Date.AddDays(-6).AddHours(23).AddMinutes(30)),
            CreateEntry(userId.Value, "Instagram", 1.6, now.Date.AddDays(-5).AddHours(21).AddMinutes(15)),
            CreateEntry(userId.Value, "YouTube", 3.1, now.Date.AddDays(-4).AddHours(0).AddMinutes(45)),
            CreateEntry(userId.Value, "Reddit", 1.2, now.Date.AddDays(-3).AddHours(1).AddMinutes(20)),
            CreateEntry(userId.Value, "TikTok", 2.8, now.Date.AddDays(-2).AddHours(22).AddMinutes(50)),
            CreateEntry(userId.Value, "Instagram", 1.1, now.Date.AddDays(-1).AddHours(13).AddMinutes(10)),
            CreateEntry(userId.Value, "YouTube", 2.2, now.Date.AddHours(20).AddMinutes(5)),
            CreateEntry(userId.Value, "Reddit", 1.7, now.Date.AddHours(2).AddMinutes(10))
        };

        context.ScreenTimeEntries.AddRange(entries);
        await context.SaveChangesAsync();

        return Ok(new
        {
            message = "Demo screen time data seeded successfully.",
            entriesCreated = entries.Count
        });
    }

    private static ScreenTimeEntry CreateEntry(Guid userId, string platformName, double hoursSpent, DateTime usageDate)
    {
        return new ScreenTimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlatformName = platformName,
            HoursSpent = hoursSpent,
            UsageDate = usageDate
        };
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
