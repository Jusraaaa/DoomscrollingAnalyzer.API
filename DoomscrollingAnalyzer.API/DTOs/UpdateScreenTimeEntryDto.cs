using System.ComponentModel.DataAnnotations;

namespace DoomscrollingAnalyzer.API.DTOs;

public class UpdateScreenTimeEntryDto
{
    [Required]
    [MaxLength(100)]
    public string PlatformName { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 24)]
    public double HoursSpent { get; set; }

    [Required]
    public DateTime UsageDate { get; set; }
}
