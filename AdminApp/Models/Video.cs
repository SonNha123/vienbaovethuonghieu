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
            // 1. Match watch?v= or &v=
            var vMatch = System.Text.RegularExpressions.Regex.Match(VideoUrl, @"[?&]v=([^#&?]{11})");
            if (vMatch.Success)
            {
                return $"https://img.youtube.com/vi/{vMatch.Groups[1].Value}/hqdefault.jpg";
            }

            // 2. Match embed/, v/, shorts/
            var pathMatch = System.Text.RegularExpressions.Regex.Match(VideoUrl, @"(?:embed|v|shorts)\/([^#&?]{11})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (pathMatch.Success)
            {
                return $"https://img.youtube.com/vi/{pathMatch.Groups[1].Value}/hqdefault.jpg";
            }

            // 3. Match youtu.be/
            var shortMatch = System.Text.RegularExpressions.Regex.Match(VideoUrl, @"youtu\.be\/([^#&?]{11})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (shortMatch.Success)
            {
                return $"https://img.youtube.com/vi/{shortMatch.Groups[1].Value}/hqdefault.jpg";
            }
        }

        return "https://images.unsplash.com/photo-1611162617474-5b21e879e113?q=80&w=400";
    }

    public bool IsFeatured { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
