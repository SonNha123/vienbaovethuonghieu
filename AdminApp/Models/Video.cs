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

    public string GetResolvedThumbnailUrl()
    {
        if (!string.IsNullOrEmpty(ThumbnailUrl))
        {
            return ThumbnailUrl;
        }

        if (!string.IsNullOrEmpty(VideoUrl))
        {
            var match = System.Text.RegularExpressions.Regex.Match(
                VideoUrl, 
                @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^""&?\/\s]{11})", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match.Success)
            {
                return $"https://img.youtube.com/vi/{match.Groups[1].Value}/hqdefault.jpg";
            }
        }

        return "https://images.unsplash.com/photo-1611162617474-5b21e879e113?q=80&w=400";
    }

    public bool IsFeatured { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
