using DoomscrollingAnalyzer.API.Models;
using DoomscrollingAnalyzer.API.Repositories;
using DoomscrollingAnalyzer.API.Tests.TestSupport;
using FluentAssertions;

namespace DoomscrollingAnalyzer.API.Tests;

public class ScreenTimeRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResultsForUser()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();

        context.ScreenTimeEntries.AddRange(
            CreateEntry(userId, "TikTok", 5, new DateTime(2026, 5, 1)),
            CreateEntry(userId, "YouTube", 4, new DateTime(2026, 5, 2)),
            CreateEntry(userId, "Reddit", 3, new DateTime(2026, 5, 3)),
            CreateEntry(userId, "Docs", 2, new DateTime(2026, 5, 4)),
            CreateEntry(userId, "Email", 1, new DateTime(2026, 5, 5)),
            CreateEntry(Guid.NewGuid(), "TikTok", 10, new DateTime(2026, 5, 6)));
        await context.SaveChangesAsync();

        var repository = new ScreenTimeRepository(context);

        var result = await repository.GetAllAsync(userId, 2, 2, null, null, "hours", "desc");

        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(3);
        result.Items.Select(entry => entry.HoursSpent).Should().Equal(3, 2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterByPlatform()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();

        context.ScreenTimeEntries.AddRange(
            CreateEntry(userId, "TikTok", 2, new DateTime(2026, 5, 1)),
            CreateEntry(userId, "TikTok", 3, new DateTime(2026, 5, 2)),
            CreateEntry(userId, "YouTube", 4, new DateTime(2026, 5, 3)),
            CreateEntry(Guid.NewGuid(), "TikTok", 10, new DateTime(2026, 5, 4)));
        await context.SaveChangesAsync();

        var repository = new ScreenTimeRepository(context);

        var result = await repository.GetAllAsync(userId, 1, 10, null, "TikTok", "date", "asc");

        result.TotalCount.Should().Be(2);
        result.Items.Should().OnlyContain(entry => entry.PlatformName == "TikTok" && entry.UserId == userId);
        result.Items.Select(entry => entry.HoursSpent).Should().Equal(2, 3);
    }

    [Fact]
    public async Task GetAllAsync_ShouldSearchByPlatformName()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();

        context.ScreenTimeEntries.AddRange(
            CreateEntry(userId, "YouTube", 2, new DateTime(2026, 5, 1)),
            CreateEntry(userId, "YouTube Shorts", 3, new DateTime(2026, 5, 2)),
            CreateEntry(userId, "Reddit", 4, new DateTime(2026, 5, 3)));
        await context.SaveChangesAsync();

        var repository = new ScreenTimeRepository(context);

        var result = await repository.GetAllAsync(userId, 1, 10, "YouTube", null, "date", "asc");

        result.TotalCount.Should().Be(2);
        result.Items.Should().OnlyContain(entry => entry.PlatformName.Contains("YouTube"));
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
