using System.ComponentModel.DataAnnotations;

namespace DoomscrollingAnalyzer.API.Models;

public class AppUser
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; }
}
