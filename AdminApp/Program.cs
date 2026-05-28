using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using New_folder.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);

// Force Admin CMS App to run on port 5001
builder.WebHost.UseUrls("http://localhost:5001");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext with SQL Server Connection String (with secure fallback)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = "Server=YOUR_DB_SERVER;Database=KDTM_BaoVeThuongHieu_New;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;";
}


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure Cookie Authentication for Admin/Editor/Partner roles
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

var app = builder.Build();

// Automatically create database and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        // 1. Dynamic SQL Migration: Add IsShownOnNav to Categories if not exists
        context.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Categories') AND name = 'IsShownOnNav')
            BEGIN
                ALTER TABLE dbo.Categories ADD IsShownOnNav BIT NOT NULL DEFAULT 1;
            END
        ");

        // 2. Dynamic SQL Migration: Create UserPermissions table if not exists
        context.Database.ExecuteSqlRaw(@"
            IF OBJECT_ID('dbo.UserPermissions', 'U') IS NULL
            BEGIN
                CREATE TABLE dbo.UserPermissions (
                    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.Users(Id) ON DELETE CASCADE,
                    ModuleName NVARCHAR(50) NOT NULL,
                    CanCreate BIT NOT NULL DEFAULT 0,
                    CanEdit BIT NOT NULL DEFAULT 0,
                    CanDelete BIT NOT NULL DEFAULT 0
                );
            END
        ");

        // 3. Dynamic SQL Migration: Add VideoUrl, VideoType, and AdditionalImages to Posts if not exists
        context.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Posts') AND name = 'VideoUrl')
            BEGIN
                ALTER TABLE dbo.Posts ADD VideoUrl NVARCHAR(500) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Posts') AND name = 'VideoType')
            BEGIN
                ALTER TABLE dbo.Posts ADD VideoType NVARCHAR(50) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Posts') AND name = 'AdditionalImages')
            BEGIN
                ALTER TABLE dbo.Posts ADD AdditionalImages NVARCHAR(MAX) NULL;
            END
        ");

        // 4. Dynamic SQL Migration: Add DisplayLayout to Categories if not exists
        context.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Categories') AND name = 'DisplayLayout')
            BEGIN
                ALTER TABLE dbo.Categories ADD DisplayLayout NVARCHAR(50) NOT NULL DEFAULT 'Standard';
            END
        ");

        // Self-healing database correction for Admin password hash
        var adminUser = context.Users.FirstOrDefault(u => u.Username == "admin");
        if (adminUser != null && adminUser.PasswordHash == "9c3c137db0f1cd0bfa8f1ad8b3ad8540c115c5443fa484cf75306ba2dfd9f4e2")
        {
            adminUser.PasswordHash = "d37e29b473701d83f8e9f00c37d22d387cce56d37671609ee5a6b643a8a8609a"; // Correct SHA256 of "Thuong@123"
            context.SaveChanges();
            Console.WriteLine("[DB INIT] Self-healed: Corrected admin password hash to match 'Thuong@123'.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable static assets for net10
app.UseStaticFiles();
app.MapStaticAssets();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
