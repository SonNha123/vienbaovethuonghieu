using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using New_folder.Models;

namespace New_folder.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // 1. Get featured posts for slider (Tiêu điểm & IsFeatured = true)
        var featuredPosts = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Author)
            .Where(p => p.IsApproved && p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .ToListAsync();

        // If no featured posts, take latest approved posts
        if (!featuredPosts.Any())
        {
            featuredPosts = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Where(p => p.IsApproved)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();
        }

        // 2. Get latest news (for sidebar widget)
        var latestNews = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsApproved)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToListAsync();

        // 3. Get all active categories with eager loaded approved posts
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsShownOnNav)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        var categoryPosts = new Dictionary<int, List<Post>>();
        foreach (var cat in categories)
        {
            var posts = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Author)
                .Where(p => p.CategoryId == cat.Id && p.IsApproved)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8) // 1 large + 2 small + 3 bottom + 2 extra
                .ToListAsync();
            categoryPosts[cat.Id] = posts;
        }

        // 4. Get active banners
        var banners = await _context.Banners
            .AsNoTracking()
            .Where(b => b.IsActive && b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now)
            .ToListAsync();

        // 5. Get active videos (for sidebar widget)
        var videos = await _context.Videos
            .AsNoTracking()
            .Where(v => v.IsActive)
            .OrderByDescending(v => v.IsFeatured)
            .ThenByDescending(v => v.CreatedAt)
            .Take(3)
            .ToListAsync();

        ViewBag.FeaturedPosts = featuredPosts;
        ViewBag.LatestNews = latestNews;
        ViewBag.CategoryPosts = categoryPosts;
        ViewBag.Banners = banners;
        ViewBag.Videos = videos;
        ViewBag.Categories = categories;

        return View();
    }

    [Route("tin-tuc/{slug}")]
    public async Task<IActionResult> Category(string slug, int page = 1)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (category == null)
            return NotFound();

        const int pageSize = 10;

        var totalPosts = await _context.Posts
            .CountAsync(p => p.CategoryId == category.Id && p.IsApproved);

        var totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);
        page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

        var posts = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Where(p => p.CategoryId == category.Id && p.IsApproved)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Sidebar: latest posts from this category (excluding current page posts)
        var sidebarPosts = await _context.Posts
            .AsNoTracking()
            .Where(p => p.CategoryId == category.Id && p.IsApproved)
            .OrderByDescending(p => p.CreatedAt)
            .Take(6)
            .ToListAsync();

        // Sidebar: active sidebar banners
        var sidebarBanners = await _context.Banners
            .AsNoTracking()
            .Where(b => b.Position == "Sidebar" && b.IsActive
                && b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now
                && !b.Title.Contains("Trái") && !b.Title.Contains("Phải"))
            .ToListAsync();

        ViewBag.Category = category;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.SidebarPosts = sidebarPosts;
        ViewBag.SidebarBanners = sidebarBanners;

        return View(posts);
    }

    [Route("bai-viet/{slug}")]
    public async Task<IActionResult> PostDetail(string slug)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsApproved);

        if (post == null)
            return NotFound();

        // Related posts from same category
        var relatedPosts = await _context.Posts
            .AsNoTracking()
            .Where(p => p.CategoryId == post.CategoryId && p.Id != post.Id && p.IsApproved)
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .ToListAsync();

        // Latest posts across all categories (for sidebar)
        var latestPosts = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsApproved && p.Id != post.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .ToListAsync();

        // Sidebar banners
        var sidebarBanners = await _context.Banners
            .AsNoTracking()
            .Where(b => b.Position == "Sidebar" && b.IsActive
                && b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now
                && !b.Title.Contains("Trái") && !b.Title.Contains("Phải"))
            .ToListAsync();

        ViewBag.RelatedPosts = relatedPosts;
        ViewBag.LatestPosts = latestPosts;
        ViewBag.SidebarBanners = sidebarBanners;

        return View(post);
    }

    public async Task<IActionResult> Search(string query)
    {
        ViewBag.Query = query;
        if (string.IsNullOrEmpty(query))
        {
            return View(new List<Post>());
        }

        var results = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Author)
            .Where(p => p.IsApproved && (p.Title.Contains(query) || p.Summary!.Contains(query) || p.Content.Contains(query)))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(results);
    }

    [Route("tat-ca-video")]
    public async Task<IActionResult> Videos(int page = 1)
    {
        const int pageSize = 12;
        
        var query = _context.Videos
            .AsNoTracking()
            .Where(v => v.IsActive);
            
        var totalVideos = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalVideos / pageSize);
        page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));
        
        var videosList = await query
            .OrderByDescending(v => v.IsFeatured)
            .ThenByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        
        return View(videosList);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
