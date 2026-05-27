using System.ComponentModel.DataAnnotations;

namespace New_folder.Models;

public class UserPermission
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public User? User { get; set; }

    [Required]
    [StringLength(50)]
    public string ModuleName { get; set; } = string.Empty;

    public bool CanCreate { get; set; } = false;

    public bool CanEdit { get; set; } = false;

    public bool CanDelete { get; set; } = false;
}
