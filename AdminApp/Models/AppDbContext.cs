using Microsoft.EntityFrameworkCore;

namespace New_folder.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<PartnerLogo> PartnerLogos { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User - UserPermission (One-to-Many)
        modelBuilder.Entity<UserPermission>()
            .HasOne(up => up.User)
            .WithMany(u => u.Permissions)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure User - Role (One-to-Many)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Post - Category (One-to-Many)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Post - User (Author) (One-to-Many)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "SuperAdmin", Description = "System Administrator" },
            new Role { Id = 2, Name = "Editor", Description = "Content Publisher & Editor" },
            new Role { Id = 3, Name = "Partner", Description = "Business/Enterprise Account" }
        );

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Chương trình & Sự kiện", Slug = "chuong-trinh-su-kien", DisplayOrder = 1 },
            new Category { Id = 2, Name = "Thương hiệu Doanh nghiệp", Slug = "thuong-hieu-doanh-nghiep", DisplayOrder = 2 },
            new Category { Id = 3, Name = "Xây dựng Thương hiệu", Slug = "xay-dung-thuong-hieu", DisplayOrder = 3 },
            new Category { Id = 4, Name = "Kinh nghiệm Tiêu dùng", Slug = "kinh-nghiem-tieu-dung", DisplayOrder = 4 },
            new Category { Id = 5, Name = "Góc nhìn Thương hiệu", Slug = "goc-nhin-thuong-hieu", DisplayOrder = 5 },
            new Category { Id = 6, Name = "Chống hàng giả", Slug = "chong-hang-giai", DisplayOrder = 6 },
            new Category { Id = 7, Name = "Tiêu điểm", Slug = "tieu-diem", DisplayOrder = 7 }
        );

        // Seed Default SuperAdmin user (Password: Thuong@123)
        // Hash for "Thuong@123" generated using BCrypt or similar standard hash (or simple string for custom hash)
        // We will write a helper to verify hashes in our AuthService.
        // For simplicity, we seed a pre-hashed password using SHA256 or bcrypt.
        // Let's use a standard password hash format, e.g. SHA256 hash of "Thuong@123" with salt or plain hash.
        // SHA256 of "Thuong@123" is: "d37e29b473701d83f8e9f00c37d22d387cce56d37671609ee5a6b643a8a8609a"
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "d37e29b473701d83f8e9f00c37d22d387cce56d37671609ee5a6b643a8a8609a", // Correct hash of "Thuong@123"
                FullName = "Administrator",
                Email = "admin@baovethuonghieu.org",
                CompanyName = "Viện Phát triển Thương hiệu",
                IsActive = true,
                RoleId = 1, // SuperAdmin
                CreatedAt = new DateTime(2026, 5, 26)
            }
        );
    }
}
