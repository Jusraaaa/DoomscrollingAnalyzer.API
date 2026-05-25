namespace DoomscrollingAnalyzer.API.DTOs;

public class ScreenTimeEntryResponseDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string PlatformName { get; set; } = string.Empty;

    public double HoursSpent { get; set; }

    public DateTime UsageDate { get; set; }
}
