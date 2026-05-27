using System.ComponentModel.DataAnnotations;

namespace New_folder.Models;

public class Banner
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(500)]
    public string? RedirectUrl { get; set; }

    [Required]
    [StringLength(50)]
    public string Position { get; set; } = "Header"; // Header, Sidebar, InBetween, Sticky

    public DateTime StartDate { get; set; } = DateTime.Now;

    public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
