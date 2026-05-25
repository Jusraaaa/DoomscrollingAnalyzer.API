namespace DoomscrollingAnalyzer.API.DTOs;

public class AnalyticsSummaryDto
{
    public double TotalHoursSpent { get; set; }

    public string MostUsedPlatform { get; set; } = string.Empty;

    public double AverageDailyUsage { get; set; }

    public double DoomscrollingScore { get; set; }

    public string ProductivityStatus { get; set; } = string.Empty;

    public string RecommendationMessage { get; set; } = string.Empty;
}
