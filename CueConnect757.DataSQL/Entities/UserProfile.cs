using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CueConnect757.DataSQL.Entities;

public class UserProfile
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(450)]
    public string UserId { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Nickname { get; set; }

    [StringLength(100)]
    public string? PreferredDisplayName { get; set; }

    [StringLength(100)]
    public string? TimeZone { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? NotificationPreferences { get; set; }

    [StringLength(500)]
    public string? ApaRefreshToken { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
