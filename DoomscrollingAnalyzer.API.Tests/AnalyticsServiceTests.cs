using DoomscrollingAnalyzer.API.Models;
using DoomscrollingAnalyzer.API.Services;
using DoomscrollingAnalyzer.API.Tests.TestSupport;
using FluentAssertions;

namespace DoomscrollingAnalyzer.API.Tests;

public class AnalyticsServiceTests
{
    [Fact]
    public async Task GetSummaryAsync_ShouldCalculateDoomscrollingScore()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();

        context.ScreenTimeEntries.AddRange(
            CreateEntry(userId, "TikTok", 2, new DateTime(2026, 5, 20, 23, 0, 0)),
            CreateEntry(userId, "Reddit", 1, new DateTime(2026, 5, 21, 1, 0, 0)),
            CreateEntry(userId, "Docs", 1, new DateTime(2026, 5, 21, 12, 0, 0)),
            CreateEntry(Guid.NewGuid(), "TikTok", 10, new DateTime(2026, 5, 21, 23, 0, 0)));
        await context.SaveChangesAsync();

        var service = new AnalyticsService(context);

        var summary = await service.GetSummaryAsync(userId);

        summary.TotalHoursSpent.Should().Be(4);
        summary.MostUsedPlatform.Should().Be("TikTok");
        summary.AverageDailyUsage.Should().Be(2);
        summary.DoomscrollingScore.Should().Be(48.25);
        summary.ProductivityStatus.Should().Be("Moderate Risk");
        summary.RecommendationMessage.Should().Contain("daily limits");
    }

    [Fact]
    public async Task GetWeeklyReportAsync_ShouldGroupEntriesByWeek()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();

        context.ScreenTimeEntries.AddRange(
            CreateEntry(userId, "YouTube", 2, new DateTime(2026, 5, 18, 10, 0, 0)),
            CreateEntry(userId, "Reddit", 3, new DateTime(2026, 5, 20, 23, 0, 0)),
            CreateEntry(userId, "Docs", 1, new DateTime(2026, 5, 11, 9, 0, 0)));
        await context.SaveChangesAsync();

        var service = new AnalyticsService(context);

        var report = await service.GetWeeklyReportAsync(userId);

        report.Should().HaveCount(2);
        report.First().WeekStartDate.Should().Be(new DateTime(2026, 5, 18));
        report.First().TotalHoursSpent.Should().Be(5);
        report.First().EntryCount.Should().Be(2);
    }

    private static ScreenTimeEntry CreateEntry(Guid userId, string platform, double hours, DateTime usageDate)
    {
        return new ScreenTimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlatformName = platform,
            HoursSpent = hours,
            UsageDate = usageDate
        };
    }
}
