using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using New_folder.Models;

namespace New_folder.Controllers;

[Authorize(Roles = "Partner")]
public class PartnerController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public PartnerController(AppDbContext context, IWebHostEnvironment env, IConfiguration config)
    {
        _context = context;
        _env = env;
        _config = config;
    }

    public async Task<IActionResult> Index()
    {
        var username = User.Identity?.Name;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return RedirectToAction("Logout", "Auth");

        // Get partner's posts
        var myPosts = await _context.Posts
            .Include(p => p.Category)
            .Where(p => p.AuthorId == user.Id)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        // Check if partner already has a logo uploaded in the partner logos table
        var logo = await _context.PartnerLogos
            .FirstOrDefaultAsync(p => p.CompanyName == user.CompanyName);

        ViewBag.User = user;
        ViewBag.Logo = logo;
        ViewBag.Categories = await _context.Categories.ToListAsync();

        return View(myPosts);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateLogo(IFormFile logoFile, string websiteUrl)
    {
        var username = User.Identity?.Name;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return RedirectToAction("Logout", "Auth");

        if (logoFile == null || string.IsNullOrEmpty(user.CompanyName))
        {
            TempData["ErrorMessage"] = "Vui lòng chọn một tệp ảnh Logo hợp lệ.";
            return RedirectToAction("Index");
        }

        // Save file locally in AdminApp
        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "logos");
        Directory.CreateDirectory(uploadsFolder);
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(logoFile.FileName);
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await logoFile.CopyToAsync(fileStream);
        }

        // Copy file to PortalApp wwwroot so it loads immediately there
        try
        {
            string portalUploadsRoot = _config["UploadPath"] ?? Path.Combine(Path.GetDirectoryName(_env.ContentRootPath)!, "PortalApp", "wwwroot", "uploads");
            string portalUploads = Path.Combine(portalUploadsRoot, "logos");
            Directory.CreateDirectory(portalUploads);
            System.IO.File.Copy(filePath, Path.Combine(portalUploads, uniqueFileName), true);
        }
        catch {}

        string logoUrl = "/uploads/logos/" + uniqueFileName;

        // Check if logo exists
        var existingLogo = await _context.PartnerLogos
            .FirstOrDefaultAsync(p => p.CompanyName == user.CompanyName);

        if (existingLogo != null)
        {
            existingLogo.LogoUrl = logoUrl;
            existingLogo.WebsiteUrl = websiteUrl;
        }
        else
        {
            var newLogo = new PartnerLogo
            {
                CompanyName = user.CompanyName,
                LogoUrl = logoUrl,
                WebsiteUrl = websiteUrl,
                DisplayOrder = 100, // Show at the back
                IsActive = true
            };
            _context.PartnerLogos.Add(newLogo);
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã cập nhật Logo doanh nghiệp thành công! Logo của bạn đã được xuất bản ở chân trang Portal.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> SubmitPR(string title, string summary, string content, int categoryId, IFormFile? imageFile)
    {
        var username = User.Identity?.Name;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return RedirectToAction("Logout", "Auth");

        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content) || categoryId <= 0)
        {
            TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ tiêu đề, chuyên mục và nội dung bài viết PR.";
            return RedirectToAction("Index");
        }

        string? imageUrl = null;
        if (imageFile != null)
        {
            // Save file locally in AdminApp
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "posts");
            Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Copy to PortalApp wwwroot so it loads immediately
            try
            {
                string portalUploadsRoot = _config["UploadPath"] ?? Path.Combine(Path.GetDirectoryName(_env.ContentRootPath)!, "PortalApp", "wwwroot", "uploads");
                string portalUploads = Path.Combine(portalUploadsRoot, "posts");
                Directory.CreateDirectory(portalUploads);
                System.IO.File.Copy(filePath, Path.Combine(portalUploads, uniqueFileName), true);
            }
            catch {}

            imageUrl = "/uploads/posts/" + uniqueFileName;
        }

        var slug = GenerateSlug(title);

        var post = new Post
        {
            Title = title,
            Slug = slug,
            Summary = summary,
            Content = content,
            ImageUrl = imageUrl,
            IsFeatured = false,
            IsApproved = false, // Business PR drafts must be approved by Editors/Admins first!
            CategoryId = categoryId,
            AuthorId = user.Id
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Gửi bài viết PR thành công! Bài viết đã chuyển đến hàng đợi của Ban biên tập để kiểm duyệt trước khi xuất bản.";
        return RedirectToAction("Index");
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
