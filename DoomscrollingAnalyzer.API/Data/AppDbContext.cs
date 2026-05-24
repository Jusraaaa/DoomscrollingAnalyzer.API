using DoomscrollingAnalyzer.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DoomscrollingAnalyzer.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> AppUsers { get; set; }

    public DbSet<ScreenTimeEntry> ScreenTimeEntries { get; set; }

    public DbSet<MoodEntry> MoodEntries { get; set; }

    public DbSet<DoomscrollingReport> DoomscrollingReports { get; set; }
}
