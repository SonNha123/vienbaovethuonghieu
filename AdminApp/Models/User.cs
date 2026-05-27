using System.ComponentModel.DataAnnotations;

namespace New_folder.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(150)]
    public string? CompanyName { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public ICollection<UserPermission> Permissions { get; set; } = new List<UserPermission>();
}
