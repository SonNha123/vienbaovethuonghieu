using System.ComponentModel.DataAnnotations;

namespace New_folder.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Slug { get; set; } = string.Empty;

    public int DisplayOrder { get; set; } = 0;

    public bool IsShownOnNav { get; set; } = true;

    [Required]
    [StringLength(50)]
    public string DisplayLayout { get; set; } = "Standard"; // "Standard", "Grid", "List", "FeaturedOnly"

    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
