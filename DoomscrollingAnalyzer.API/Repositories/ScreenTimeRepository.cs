using DoomscrollingAnalyzer.API.Data;
using DoomscrollingAnalyzer.API.DTOs;
using DoomscrollingAnalyzer.API.Interfaces;
using DoomscrollingAnalyzer.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DoomscrollingAnalyzer.API.Repositories;

public class ScreenTimeRepository(AppDbContext context) : IScreenTimeRepository
{
    public async Task<ScreenTimeEntry> CreateAsync(ScreenTimeEntry entry)
    {
        context.ScreenTimeEntries.Add(entry);
        await context.SaveChangesAsync();

        return entry;
    }

    public async Task<PagedResponseDto<ScreenTimeEntry>> GetAllAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? search,
        string? platform,
        string? sortBy,
        string? sortDirection)
    {
        var query = context.ScreenTimeEntries
            .AsNoTracking()
            .Where(entry => entry.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();
            query = query.Where(entry => entry.PlatformName.Contains(normalizedSearch));
        }

        if (!string.IsNullOrWhiteSpace(platform))
        {
            var normalizedPlatform = platform.Trim();
            query = query.Where(entry => entry.PlatformName == normalizedPlatform);
        }

        query = ApplySorting(query, sortBy, sortDirection);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponseDto<ScreenTimeEntry>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public Task<ScreenTimeEntry?> GetByIdAsync(Guid id, Guid userId)
    {
        return context.ScreenTimeEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(entry => entry.Id == id && entry.UserId == userId);
    }

    public async Task<bool> UpdateAsync(ScreenTimeEntry entry)
    {
        var existingEntry = await context.ScreenTimeEntries
            .FirstOrDefaultAsync(screenTimeEntry => screenTimeEntry.Id == entry.Id && screenTimeEntry.UserId == entry.UserId);

        if (existingEntry is null)
        {
            return false;
        }

        existingEntry.PlatformName = entry.PlatformName;
        existingEntry.HoursSpent = entry.HoursSpent;
        existingEntry.UsageDate = entry.UsageDate;

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var entry = await context.ScreenTimeEntries
            .FirstOrDefaultAsync(screenTimeEntry => screenTimeEntry.Id == id && screenTimeEntry.UserId == userId);

        if (entry is null)
        {
            return false;
        }

        context.ScreenTimeEntries.Remove(entry);
        await context.SaveChangesAsync();

        return true;
    }

    private static IQueryable<ScreenTimeEntry> ApplySorting(
        IQueryable<ScreenTimeEntry> query,
        string? sortBy,
        string? sortDirection)
    {
        var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "hours" or "hoursspent" => descending
                ? query.OrderByDescending(entry => entry.HoursSpent)
                : query.OrderBy(entry => entry.HoursSpent),
            "date" or "usagedate" => descending
                ? query.OrderByDescending(entry => entry.UsageDate)
                : query.OrderBy(entry => entry.UsageDate),
            _ => query.OrderByDescending(entry => entry.UsageDate)
        };
    }
}
