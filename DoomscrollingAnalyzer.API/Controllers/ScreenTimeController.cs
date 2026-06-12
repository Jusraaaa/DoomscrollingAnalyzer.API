using System.Security.Claims;
using AutoMapper;
using DoomscrollingAnalyzer.API.DTOs;
using DoomscrollingAnalyzer.API.Interfaces;
using DoomscrollingAnalyzer.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoomscrollingAnalyzer.API.Controllers;

[ApiController]
[Authorize]
[Route("api/screentime")]
public class ScreenTimeController(IScreenTimeRepository screenTimeRepository, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ScreenTimeEntryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ScreenTimeEntryResponseDto>> Create(CreateScreenTimeEntryDto request)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var entry = mapper.Map<ScreenTimeEntry>(request);
        entry.Id = Guid.NewGuid();
        entry.UserId = userId.Value;

        var createdEntry = await screenTimeRepository.CreateAsync(entry);
        var response = mapper.Map<ScreenTimeEntryResponseDto>(createdEntry);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<ScreenTimeEntryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponseDto<ScreenTimeEntryResponseDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? platform = null,
        [FromQuery] string? sortBy = "date",
        [FromQuery] string? sortDirection = "desc")
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var pagedEntries = await screenTimeRepository.GetAllAsync(
            userId.Value,
            pageNumber,
            pageSize,
            search,
            platform,
            sortBy,
            sortDirection);

        return Ok(new PagedResponseDto<ScreenTimeEntryResponseDto>
        {
            Items = mapper.Map<IReadOnlyCollection<ScreenTimeEntryResponseDto>>(pagedEntries.Items),
            PageNumber = pagedEntries.PageNumber,
            PageSize = pagedEntries.PageSize,
            TotalCount = pagedEntries.TotalCount,
            TotalPages = pagedEntries.TotalPages
        });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ScreenTimeEntryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScreenTimeEntryResponseDto>> GetById(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var entry = await screenTimeRepository.GetByIdAsync(id, userId.Value);
        if (entry is null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<ScreenTimeEntryResponseDto>(entry));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateScreenTimeEntryDto request)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var entry = mapper.Map<ScreenTimeEntry>(request);
        entry.Id = id;
        entry.UserId = userId.Value;

        var updated = await screenTimeRepository.UpdateAsync(entry);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var deleted = await screenTimeRepository.DeleteAsync(id, userId.Value);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
