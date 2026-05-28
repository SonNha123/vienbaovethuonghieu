using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using New_folder.Models;

namespace New_folder.Controllers;

[Authorize(Roles = "SuperAdmin,Editor")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public AdminController(AppDbContext context, IWebHostEnvironment env, IConfiguration config)
    {
        _context = context;
        _env = env;
        _config = config;
    }

    // ==========================================
    // PERMISSION CHECK HELPERS
    // ==========================================
    private async Task<bool> HasPermission(string module, string action)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return false;

        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || !user.IsActive) return false;

        // SuperAdmin has bypass full permissions
        if (user.Role?.Name == "SuperAdmin") return true;

        // Check specific granular permissions
        var perm = user.Permissions.FirstOrDefault(p => p.ModuleName == module);
        if (perm == null) return false;

        if (action == "Create") return perm.CanCreate;
        if (action == "Edit") return perm.CanEdit;
        if (action == "Delete") return perm.CanDelete;

        return false;
    }

    private IActionResult NoPermissionRedirect()
    {
        TempData["ErrorMessage"] = "Bạn không có quyền thực hiện hành động này. Vui lòng liên hệ SuperAdmin.";
        return RedirectToAction("AccessDenied", "Auth");
    }

    // ==========================================
    // 1. DASHBOARD OVERVIEW
    // ==========================================
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalPosts = await _context.Posts.CountAsync();
        ViewBag.PendingPosts = await _context.Posts.CountAsync(p => !p.IsApproved);
        ViewBag.TotalBanners = await _context.Banners.CountAsync();
        ViewBag.TotalPartners = await _context.PartnerLogos.CountAsync();
        ViewBag.TotalVideos = await _context.Videos.CountAsync();
        ViewBag.TotalUsers = await _context.Users.CountAsync();

        // Get recent pending articles
        var recentPending = await _context.Posts
            .Include(p => p.Category)
            .Include(p => p.Author)
            .Where(p => !p.IsApproved)
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .ToListAsync();

        return View(recentPending);
    }

    // ==========================================
    // 2. BANNER ADS MANAGEMENT (CRUD)
    // ==========================================
    public async Task<IActionResult> Banners()
    {
        if (!await HasPermission("Banners", "Create") && !await HasPermission("Banners", "Edit") && !await HasPermission("Banners", "Delete"))
            return RedirectToAction("AccessDenied", "Auth");

        var list = await _context.Banners
            .Where(b => b.Position != "Header")
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
        return View(list);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBanner(string title, string redirectUrl, string position, DateTime startDate, DateTime endDate, IFormFile? imageFile)
    {
        if (!await HasPermission("Banners", "Create")) return NoPermissionRedirect();

        if (string.IsNullOrEmpty(title) || imageFile == null)
        {
            TempData["ErrorMessage"] = "Vui lòng điền tiêu đề và tải lên ảnh banner.";
            return RedirectToAction("Banners");
        }

        string imageUrl = await SaveUploadedFile(imageFile, "banners");

        var banner = new Banner
        {
            Title = title,
            ImageUrl = imageUrl,
            RedirectUrl = redirectUrl,
            Position = position,
            StartDate = startDate,
            EndDate = endDate,
            IsActive = true
        };

        _context.Banners.Add(banner);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã thêm mới banner quảng cáo thành công!";
        return RedirectToAction("Banners");
    }

    [HttpPost]
    public async Task<IActionResult> EditBanner(int id, string title, string redirectUrl, string position, DateTime startDate, DateTime endDate, bool isActive, IFormFile? imageFile)
    {
        if (!await HasPermission("Banners", "Edit")) return NoPermissionRedirect();

        var banner = await _context.Banners.FindAsync(id);
        if (banner == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy banner.";
            return RedirectToAction("Banners");
        }

        if (string.IsNullOrEmpty(title))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập tiêu đề banner.";
            return RedirectToAction("Banners");
        }

        banner.Title = title;
        banner.RedirectUrl = redirectUrl;
        banner.Position = position;
        banner.StartDate = startDate;
        banner.EndDate = endDate;
        banner.IsActive = isActive;

        if (imageFile != null)
        {
            banner.ImageUrl = await SaveUploadedFile(imageFile, "banners");
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã cập nhật banner quảng cáo thành công!";
        return RedirectToAction("Banners");
    }

    public async Task<IActionResult> DeleteBanner(int id)
    {
        if (!await HasPermission("Banners", "Delete")) return NoPermissionRedirect();

        var banner = await _context.Banners.FindAsync(id);
        if (banner != null)
        {
            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa banner quảng cáo thành công.";
        }
        return RedirectToAction("Banners");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleBanner(int id)
    {
        if (!await HasPermission("Banners", "Edit")) return Json(new { success = false, message = "Không có quyền sửa banner." });

        var banner = await _context.Banners.FindAsync(id);
        if (banner != null)
        {
            banner.IsActive = !banner.IsActive;
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        return Json(new { success = false });
    }

    // ==========================================
    // 3. VIDEO MANAGEMENT (CRUD)
    // ==========================================
    public async Task<IActionResult> Videos()
    {
        if (!await HasPermission("Videos", "Create") && !await HasPermission("Videos", "Edit") && !await HasPermission("Videos", "Delete"))
            return RedirectToAction("AccessDenied", "Auth");

        var list = await _context.Videos.OrderByDescending(v => v.CreatedAt).ToListAsync();
        return View(list);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVideo(string title, string videoUrl, IFormFile? thumbnailFile)
    {
        if (!await HasPermission("Videos", "Create")) return NoPermissionRedirect();

        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(videoUrl))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ tiêu đề và liên kết phát video.";
            return RedirectToAction("Videos");
        }

        string? thumbnailUrl = null;
        if (thumbnailFile != null)
        {
            thumbnailUrl = await SaveUploadedFile(thumbnailFile, "videos");
        }

        var video = new Video
        {
            Title = title,
            VideoUrl = videoUrl,
            ThumbnailUrl = thumbnailUrl,
            IsActive = true
        };

        _context.Videos.Add(video);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã thêm mới video PR thành công!";
        return RedirectToAction("Videos");
    }

    [HttpPost]
    public async Task<IActionResult> EditVideo(int id, string title, string videoUrl, bool isActive, IFormFile? thumbnailFile)
    {
        if (!await HasPermission("Videos", "Edit")) return NoPermissionRedirect();

        var video = await _context.Videos.FindAsync(id);
        if (video == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy video.";
            return RedirectToAction("Videos");
        }

        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(videoUrl))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ tiêu đề và liên kết video.";
            return RedirectToAction("Videos");
        }

        video.Title = title;
        video.VideoUrl = videoUrl;
        video.IsActive = isActive;

        if (thumbnailFile != null)
        {
            video.ThumbnailUrl = await SaveUploadedFile(thumbnailFile, "videos");
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã cập nhật video PR thành công!";
        return RedirectToAction("Videos");
    }

    public async Task<IActionResult> DeleteVideo(int id)
    {
        if (!await HasPermission("Videos", "Delete")) return NoPermissionRedirect();

        var video = await _context.Videos.FindAsync(id);
        if (video != null)
        {
            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa video PR thành công.";
        }
        return RedirectToAction("Videos");
    }

    // ==========================================
    // 4. PARTNER LOGO MANAGEMENT (CRUD)
    // ==========================================
    public async Task<IActionResult> PartnerLogos()
    {
        if (!await HasPermission("PartnerLogos", "Create") && !await HasPermission("PartnerLogos", "Edit") && !await HasPermission("PartnerLogos", "Delete"))
            return RedirectToAction("AccessDenied", "Auth");

        var list = await _context.PartnerLogos.OrderBy(p => p.DisplayOrder).ToListAsync();
        return View(list);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePartnerLogo(string companyName, string websiteUrl, int displayOrder, IFormFile? logoFile)
    {
        if (!await HasPermission("PartnerLogos", "Create")) return NoPermissionRedirect();

        if (string.IsNullOrEmpty(companyName) || logoFile == null)
        {
            TempData["ErrorMessage"] = "Vui lòng điền tên doanh nghiệp và tải lên tệp ảnh logo.";
            return RedirectToAction("PartnerLogos");
        }

        string logoUrl = await SaveUploadedFile(logoFile, "logos");

        var partner = new PartnerLogo
        {
            CompanyName = companyName,
            LogoUrl = logoUrl,
            WebsiteUrl = websiteUrl,
            DisplayOrder = displayOrder,
            IsActive = true
        };

        _context.PartnerLogos.Add(partner);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã thêm logo doanh nghiệp đối tác thành công!";
        return RedirectToAction("PartnerLogos");
    }

    [HttpPost]
    public async Task<IActionResult> EditPartnerLogo(int id, string companyName, string websiteUrl, int displayOrder, bool isActive, IFormFile? logoFile)
    {
        if (!await HasPermission("PartnerLogos", "Edit")) return NoPermissionRedirect();

        var partner = await _context.PartnerLogos.FindAsync(id);
        if (partner == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy đối tác.";
            return RedirectToAction("PartnerLogos");
        }

        if (string.IsNullOrEmpty(companyName))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập tên doanh nghiệp.";
            return RedirectToAction("PartnerLogos");
        }

        partner.CompanyName = companyName;
        partner.WebsiteUrl = websiteUrl;
        partner.DisplayOrder = displayOrder;
        partner.IsActive = isActive;

        if (logoFile != null)
        {
            partner.LogoUrl = await SaveUploadedFile(logoFile, "logos");
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã cập nhật đối tác thành công!";
        return RedirectToAction("PartnerLogos");
    }

    public async Task<IActionResult> DeletePartnerLogo(int id)
    {
        if (!await HasPermission("PartnerLogos", "Delete")) return NoPermissionRedirect();

        var partner = await _context.PartnerLogos.FindAsync(id);
        if (partner != null)
        {
            _context.PartnerLogos.Remove(partner);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa logo doanh nghiệp đối tác thành công.";
        }
        return RedirectToAction("PartnerLogos");
    }

    // ==========================================
    // 5. POSTS MANAGEMENT (DUYỆT & ĐĂNG BÀI)
    // ==========================================
    public async Task<IActionResult> Posts()
    {
        if (!await HasPermission("Posts", "Create") && !await HasPermission("Posts", "Edit") && !await HasPermission("Posts", "Delete"))
            return RedirectToAction("AccessDenied", "Auth");

        var list = await _context.Posts
            .Include(p => p.Category)
            .Include(p => p.Author)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(list);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost(string title, string summary, string content, int categoryId, bool isFeatured, IFormFile? imageFile)
    {
        if (!await HasPermission("Posts", "Create")) return NoPermissionRedirect();

        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content) || categoryId <= 0)
        {
            TempData["ErrorMessage"] = "Vui lòng điền đầy đủ tiêu đề, chuyên mục và nội dung bài viết.";
            return RedirectToAction("Posts");
        }

        string? imageUrl = null;
        if (imageFile != null)
        {
            imageUrl = await SaveUploadedFile(imageFile, "posts");
        }

        var username = User.Identity?.Name;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return RedirectToAction("Logout", "Auth");

        var slug = GenerateSlug(title);

        var post = new Post
        {
            Title = title,
            Slug = slug,
            Summary = summary,
            Content = content,
            ImageUrl = imageUrl,
            IsFeatured = isFeatured,
            IsApproved = true, // Admins' and Editors' posts are automatically approved!
            CategoryId = categoryId,
            AuthorId = user.Id
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã đăng bài viết mới thành công!";
        return RedirectToAction("Posts");
    }

    [HttpPost]
    public async Task<IActionResult> EditPost(int id, string title, string summary, string content, int categoryId, bool isFeatured, bool isApproved, IFormFile? imageFile)
    {
        if (!await HasPermission("Posts", "Edit")) return NoPermissionRedirect();

        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy bài viết.";
            return RedirectToAction("Posts");
        }

        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content) || categoryId <= 0)
        {
            TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ tiêu đề, chuyên mục và nội dung.";
            return RedirectToAction("Posts");
        }

        post.Title = title;
        post.Slug = GenerateSlug(title);
        post.Summary = summary;
        post.Content = content;
        post.CategoryId = categoryId;
        post.IsFeatured = isFeatured;
        post.IsApproved = isApproved;

        if (imageFile != null)
        {
            post.ImageUrl = await SaveUploadedFile(imageFile, "posts");
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã cập nhật bài viết thành công!";
        return RedirectToAction("Posts");
    }

    [HttpPost]
    public async Task<IActionResult> ApprovePost(int id)
    {
        if (!await HasPermission("Posts", "Edit")) return Json(new { success = false, message = "Không có quyền phê duyệt bài viết." });

        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            post.IsApproved = !post.IsApproved;
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        return Json(new { success = false });
    }

    public async Task<IActionResult> DeletePost(int id)
    {
        if (!await HasPermission("Posts", "Delete")) return NoPermissionRedirect();

        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa bài viết thành công.";
        }
        return RedirectToAction("Posts");
    }

    // ==========================================
    // 6. CATEGORIES MANAGEMENT (CRUD)
    // ==========================================
    public async Task<IActionResult> Categories()
    {
        if (!await HasPermission("Categories", "Create") && !await HasPermission("Categories", "Edit") && !await HasPermission("Categories", "Delete"))
            return RedirectToAction("AccessDenied", "Auth");

        var list = await _context.Categories.OrderBy(c => c.DisplayOrder).ToListAsync();
        return View(list);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(string name, int displayOrder, bool isShownOnNav)
    {
        if (!await HasPermission("Categories", "Create")) return NoPermissionRedirect();

        if (string.IsNullOrEmpty(name))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập tên chuyên mục.";
            return RedirectToAction("Categories");
        }

        var slug = GenerateSlug(name);

        var category = new Category
        {
            Name = name,
            Slug = slug,
            DisplayOrder = displayOrder,
            IsShownOnNav = isShownOnNav
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã thêm chuyên mục mới thành công!";
        return RedirectToAction("Categories");
    }

    [HttpPost]
    public async Task<IActionResult> EditCategory(int id, string name, int displayOrder, bool isShownOnNav)
    {
        if (!await HasPermission("Categories", "Edit")) return NoPermissionRedirect();

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy chuyên mục.";
            return RedirectToAction("Categories");
        }

        if (string.IsNullOrEmpty(name))
        {
            TempData["ErrorMessage"] = "Tên chuyên mục không được để trống.";
            return RedirectToAction("Categories");
        }

        category.Name = name;
        category.Slug = GenerateSlug(name);
        category.DisplayOrder = displayOrder;
        category.IsShownOnNav = isShownOnNav;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã cập nhật chuyên mục thành công!";
        return RedirectToAction("Categories");
    }

    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (!await HasPermission("Categories", "Delete")) return NoPermissionRedirect();

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy chuyên mục.";
            return RedirectToAction("Categories");
        }

        var hasPosts = await _context.Posts.AnyAsync(p => p.CategoryId == id);
        if (hasPosts)
        {
            TempData["ErrorMessage"] = "Không thể xóa chuyên mục này vì đang có bài viết thuộc chuyên mục.";
            return RedirectToAction("Categories");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã xóa chuyên mục thành công.";
        return RedirectToAction("Categories");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleCategoryNav(int id)
    {
        if (!await HasPermission("Categories", "Edit")) return Json(new { success = false, message = "Không có quyền sửa chuyên mục." });

        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            category.IsShownOnNav = !category.IsShownOnNav;
            await _context.SaveChangesAsync();
            return Json(new { success = true, isShownOnNav = category.IsShownOnNav });
        }
        return Json(new { success = false, message = "Không tìm thấy chuyên mục." });
    }

    // ==========================================
    // 7. DETAILED USERS & PERMISSIONS MANAGEMENT
    // ==========================================
    public async Task<IActionResult> Users()
    {
        var username = User.Identity?.Name;
        var currentUser = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);
        if (currentUser == null || currentUser.Role?.Name != "SuperAdmin")
        {
            return RedirectToAction("AccessDenied", "Auth");
        }

        var list = await _context.Users.Include(u => u.Role).ToListAsync();
        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserPermissions(int userId)
    {
        var perms = await _context.UserPermissions
            .Where(p => p.UserId == userId)
            .ToListAsync();
        return Json(new { success = true, data = perms });
    }

    [HttpPost]
    public async Task<IActionResult> EditUserPermissions(int userId, int roleId)
    {
        var username = User.Identity?.Name;
        var currentUser = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);
        if (currentUser == null || currentUser.Role?.Name != "SuperAdmin")
        {
            return RedirectToAction("AccessDenied", "Auth");
        }

        var targetUser = await _context.Users.FindAsync(userId);
        if (targetUser == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
            return RedirectToAction("Users");
        }

        targetUser.RoleId = roleId;

        var existingPerms = await _context.UserPermissions.Where(p => p.UserId == userId).ToListAsync();
        _context.UserPermissions.RemoveRange(existingPerms);

        var modules = new[] { "Posts", "Categories", "Banners", "Videos", "PartnerLogos" };
        foreach (var m in modules)
        {
            var canCreate = Request.Form[$"perm_{m}_Create"] == "true";
            var canEdit = Request.Form[$"perm_{m}_Edit"] == "true";
            var canDelete = Request.Form[$"perm_{m}_Delete"] == "true";

            if (canCreate || canEdit || canDelete)
            {
                var p = new UserPermission
                {
                    UserId = userId,
                    ModuleName = m,
                    CanCreate = canCreate,
                    CanEdit = canEdit,
                    CanDelete = canDelete
                };
                _context.UserPermissions.Add(p);
            }
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã cập nhật phân quyền tài khoản thành công!";
        return RedirectToAction("Users");
    }

    // ==========================================
    // HELPERS & FILE UPLOAD
    // ==========================================
    private async Task<string> SaveUploadedFile(IFormFile file, string folder)
    {
        string adminUploads = Path.Combine(_env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(adminUploads);

        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
        string adminFilePath = Path.Combine(adminUploads, uniqueFileName);

        using (var fileStream = new FileStream(adminFilePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        try
        {
            string portalUploadsRoot = _config["UploadPath"] ?? Path.Combine(Path.GetDirectoryName(_env.ContentRootPath)!, "PortalApp", "wwwroot", "uploads");
            string portalUploads = Path.Combine(portalUploadsRoot, folder);
            Directory.CreateDirectory(portalUploads);
            string portalFilePath = Path.Combine(portalUploads, uniqueFileName);
            System.IO.File.Copy(adminFilePath, portalFilePath, true);
        }
        catch {}

        return "/uploads/" + folder + "/" + uniqueFileName;
    }

    private string GenerateSlug(string phrase)
    {
        string str = phrase.ToLower();
        string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ", "đ", "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ", "í", "ì", "ỉ", "ĩ", "ị", "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ", "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự", "ý", "ỳ", "ỷ", "ỹ", "ỵ" };
        string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "d", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "i", "i", "i", "i", "i", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "y", "y", "y", "y", "y" };
        for (int i = 0; i < arr1.Length; i++)
        {
            str = str.Replace(arr1[i], arr2[i]);
        }
        str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ").Trim();
        str = str.Replace(" ", "-");
        return str + "-" + new Random().Next(100, 999);
    }
}
