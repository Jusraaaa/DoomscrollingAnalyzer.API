using DoomscrollingAnalyzer.API.Data;
using DoomscrollingAnalyzer.API.DTOs;
using DoomscrollingAnalyzer.API.Interfaces;
using DoomscrollingAnalyzer.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DoomscrollingAnalyzer.API.Services;

public class AnalyticsService(AppDbContext context) : IAnalyticsService
{
    private static readonly HashSet<string> HighRiskPlatforms = new(StringComparer.OrdinalIgnoreCase)
    {
        "TikTok",
        "Instagram",
        "Facebook",
        "X",
        "Twitter",
        "Reddit",
        "YouTube",
        "Shorts",
        "Reels"
    };

    public async Task<AnalyticsSummaryDto> GetSummaryAsync(Guid userId)
    {
        var entries = await context.ScreenTimeEntries
            .AsNoTracking()
            .Where(entry => entry.UserId == userId)
            .ToListAsync();

        var score = CalculateDoomscrollingScore(entries);
        var totalHours = entries.Sum(entry => entry.HoursSpent);

        return new AnalyticsSummaryDto
        {
            TotalHoursSpent = Math.Round(totalHours, 2),
            MostUsedPlatform = GetMostUsedPlatform(entries),
            AverageDailyUsage = Math.Round(CalculateAverageDailyUsage(entries), 2),
            DoomscrollingScore = score,
            ProductivityStatus = GetProductivityStatus(score),
            RecommendationMessage = GetRecommendationMessage(score)
        };
    }

    public async Task<IReadOnlyCollection<WeeklyAnalyticsDto>> GetWeeklyReportAsync(Guid userId)
    {
        var entries = await context.ScreenTimeEntries
            .AsNoTracking()
            .Where(entry => entry.UserId == userId)
            .ToListAsync();

        return entries
            .GroupBy(entry => GetWeekStartDate(entry.UsageDate))
            .OrderByDescending(group => group.Key)
            .Select(group =>
            {
                var weeklyEntries = group.ToList();
                var score = CalculateDoomscrollingScore(weeklyEntries);

                return new WeeklyAnalyticsDto
                {
                    WeekStartDate = group.Key,
                    WeekEndDate = group.Key.AddDays(6),
                    TotalHoursSpent = Math.Round(weeklyEntries.Sum(entry => entry.HoursSpent), 2),
                    AverageDailyUsage = Math.Round(CalculateAverageDailyUsage(weeklyEntries), 2),
                    EntryCount = weeklyEntries.Count,
                    MostUsedPlatform = GetMostUsedPlatform(weeklyEntries),
                    DoomscrollingScore = score,
                    ProductivityStatus = GetProductivityStatus(score)
                };
            })
            .ToList();
    }

    private static double CalculateDoomscrollingScore(IReadOnlyCollection<ScreenTimeEntry> entries)
    {
        if (entries.Count == 0)
        {
            return 0;
        }

        var totalHours = entries.Sum(entry => entry.HoursSpent);
        var lateNightHours = entries
            .Where(entry => IsLateNightUsage(entry.UsageDate))
            .Sum(entry => entry.HoursSpent);
        var highRiskPlatformHours = entries
            .Where(entry => HighRiskPlatforms.Contains(entry.PlatformName))
            .Sum(entry => entry.HoursSpent);

        var totalHoursScore = Math.Min(totalHours * 4, 40);
        var lateNightScore = Math.Min(lateNightHours * 5, 25);
        var entryFrequencyScore = Math.Min(entries.Count * 2, 20);
        var platformScore = totalHours > 0
            ? Math.Min((highRiskPlatformHours / totalHours) * 15, 15)
            : 0;

        return Math.Round(Math.Min(totalHoursScore + lateNightScore + entryFrequencyScore + platformScore, 100), 2);
    }

    private static double CalculateAverageDailyUsage(IReadOnlyCollection<ScreenTimeEntry> entries)
    {
        if (entries.Count == 0)
        {
            return 0;
        }

        var dayCount = entries
            .Select(entry => entry.UsageDate.Date)
            .Distinct()
            .Count();

        return dayCount == 0 ? 0 : entries.Sum(entry => entry.HoursSpent) / dayCount;
    }

    private static string GetMostUsedPlatform(IReadOnlyCollection<ScreenTimeEntry> entries)
    {
        return entries
            .GroupBy(entry => entry.PlatformName)
            .OrderByDescending(group => group.Sum(entry => entry.HoursSpent))
            .Select(group => group.Key)
            .FirstOrDefault() ?? "No usage recorded";
    }

    private static string GetProductivityStatus(double doomscrollingScore)
    {
        return doomscrollingScore switch
        {
            < 30 => "Healthy",
            < 60 => "Moderate Risk",
            < 80 => "High Risk",
            _ => "Critical"
        };
    }

    private static string GetRecommendationMessage(double doomscrollingScore)
    {
        return doomscrollingScore switch
        {
            < 30 => "Your screen time patterns look balanced. Keep monitoring your usage consistently.",
            < 60 => "Consider setting daily limits and moving social media usage away from late evening hours.",
            < 80 => "Your usage suggests a high doomscrolling risk. Reduce high-risk platforms and schedule offline breaks.",
            _ => "Your doomscrolling risk is critical. Prioritize app limits, notification control, and a structured digital detox plan."
        };
    }

    private static bool IsLateNightUsage(DateTime usageDate)
    {
        var hour = usageDate.Hour;

        return hour >= 22 || hour < 5;
    }

    private static DateTime GetWeekStartDate(DateTime date)
    {
        var normalizedDate = date.Date;
        var offset = ((int)normalizedDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;

        return normalizedDate.AddDays(-offset);
    }
}
