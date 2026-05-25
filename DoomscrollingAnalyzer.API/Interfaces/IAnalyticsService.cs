using DoomscrollingAnalyzer.API.DTOs;

namespace DoomscrollingAnalyzer.API.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsSummaryDto> GetSummaryAsync(Guid userId);

    Task<IReadOnlyCollection<WeeklyAnalyticsDto>> GetWeeklyReportAsync(Guid userId);
}
