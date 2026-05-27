using System.ComponentModel.DataAnnotations;

namespace New_folder.Models;

public class PartnerLogo
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string LogoUrl { get; set; } = string.Empty;

    [StringLength(500)]
    public string? WebsiteUrl { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
