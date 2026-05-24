using System.ComponentModel.DataAnnotations;

namespace DoomscrollingAnalyzer.API.Models;

public class ScreenTimeEntry
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string PlatformName { get; set; } = string.Empty;

    [Required]
    public double HoursSpent { get; set; }

    [Required]
    public DateTime UsageDate { get; set; }
}
