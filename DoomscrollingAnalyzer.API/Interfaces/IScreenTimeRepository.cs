using DoomscrollingAnalyzer.API.DTOs;
using DoomscrollingAnalyzer.API.Models;

namespace DoomscrollingAnalyzer.API.Interfaces;

public interface IScreenTimeRepository
{
    Task<ScreenTimeEntry> CreateAsync(ScreenTimeEntry entry);

    Task<PagedResponseDto<ScreenTimeEntry>> GetAllAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? search,
        string? platform,
        string? sortBy,
        string? sortDirection);

    Task<ScreenTimeEntry?> GetByIdAsync(Guid id, Guid userId);

    Task<bool> DeleteAsync(Guid id, Guid userId);
}
