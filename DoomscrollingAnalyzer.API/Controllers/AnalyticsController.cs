using System.Security.Claims;
using DoomscrollingAnalyzer.API.DTOs;
using DoomscrollingAnalyzer.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoomscrollingAnalyzer.API.Controllers;

[ApiController]
[Authorize]
[Route("api/analytics")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    [HttpGet("summary")]
    [ProducesResponseType(typeof(AnalyticsSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AnalyticsSummaryDto>> GetSummary()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var summary = await analyticsService.GetSummaryAsync(userId.Value);

        return Ok(summary);
    }

    [HttpGet("weekly-report")]
    [ProducesResponseType(typeof(IReadOnlyCollection<WeeklyAnalyticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<WeeklyAnalyticsDto>>> GetWeeklyReport()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var weeklyReport = await analyticsService.GetWeeklyReportAsync(userId.Value);

        return Ok(weeklyReport);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
