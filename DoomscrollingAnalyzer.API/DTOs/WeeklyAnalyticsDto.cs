namespace DoomscrollingAnalyzer.API.DTOs;

public class WeeklyAnalyticsDto
{
    public DateTime WeekStartDate { get; set; }

    public DateTime WeekEndDate { get; set; }

    public double TotalHoursSpent { get; set; }

    public double AverageDailyUsage { get; set; }

    public int EntryCount { get; set; }

    public string MostUsedPlatform { get; set; } = string.Empty;

    public double DoomscrollingScore { get; set; }

    public string ProductivityStatus { get; set; } = string.Empty;
}
