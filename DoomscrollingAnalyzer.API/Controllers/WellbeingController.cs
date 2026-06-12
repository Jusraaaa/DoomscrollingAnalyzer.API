using DoomscrollingAnalyzer.API.DTOs;
using DoomscrollingAnalyzer.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoomscrollingAnalyzer.API.Controllers;

[ApiController]
[Authorize]
[Route("api/wellbeing")]
public class WellbeingController(IWellbeingTipService wellbeingTipService) : ControllerBase
{
    [HttpGet("tip")]
    [ProducesResponseType(typeof(WellbeingTipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WellbeingTipDto>> GetTip(CancellationToken cancellationToken)
    {
        var tip = await wellbeingTipService.GetTipAsync(cancellationToken);

        return Ok(tip);
    }
}
