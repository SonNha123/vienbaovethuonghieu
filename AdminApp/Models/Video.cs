using System.ComponentModel.DataAnnotations;

namespace New_folder.Models;

public class Video
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string VideoUrl { get; set; } = string.Empty; // YouTube link or direct MP4 link

    [StringLength(500)]
    public string? ThumbnailUrl { get; set; }

    public bool IsFeatured { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
