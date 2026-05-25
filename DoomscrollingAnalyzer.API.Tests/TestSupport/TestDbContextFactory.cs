using DoomscrollingAnalyzer.API.Data;
using Microsoft.EntityFrameworkCore;

namespace DoomscrollingAnalyzer.API.Tests.TestSupport;

internal static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
