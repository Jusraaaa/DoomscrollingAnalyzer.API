using System.ComponentModel.DataAnnotations;

namespace DoomscrollingAnalyzer.API.Models;

public class DoomscrollingReport
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public double DoomscrollingScore { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Recommendation { get; set; } = string.Empty;

    [Required]
    public DateTime GeneratedAt { get; set; }
}
