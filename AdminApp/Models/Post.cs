using System.ComponentModel.DataAnnotations;

namespace New_folder.Models;

public class Post
{
    public int Id { get; set; }

    [Required]
    [StringLength(250)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string Slug { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Summary { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [StringLength(500)]
    public string? VideoUrl { get; set; }

    [StringLength(50)]
    public string? VideoType { get; set; } // "None", "YouTube", "Local"

    public string? AdditionalImages { get; set; } // Comma-separated or JSON list of additional image URLs

    public bool IsFeatured { get; set; } = false;

    public bool IsApproved { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int AuthorId { get; set; }
    public User? Author { get; set; }
}
