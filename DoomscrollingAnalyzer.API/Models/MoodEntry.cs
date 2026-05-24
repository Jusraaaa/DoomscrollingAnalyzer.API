using System.ComponentModel.DataAnnotations;

namespace DoomscrollingAnalyzer.API.Models;

public class MoodEntry
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Mood { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; }
}
