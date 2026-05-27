using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using New_folder.Models;

namespace New_folder.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToDashboard();
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Vui lòng nhập đầy đủ tên tài khoản và mật khẩu.");
            return View();
        }

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null || !SecurityHelper.VerifyPassword(password, user.PasswordHash))
        {
            ModelState.AddModelError("", "Tên tài khoản hoặc mật khẩu không chính xác.");
            return View();
        }

        // Establish Cookie Claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.FullName),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "Partner"),
            new Claim("UserId", user.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        return RedirectToDashboard(user.Role?.Name);
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToDashboard();
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, string password, string fullName, string email, string companyName)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(companyName))
        {
            ModelState.AddModelError("", "Vui lòng nhập đầy đủ các thông tin bắt buộc (*).");
            return View();
        }

        var existingUser = await _context.Users.AnyAsync(u => u.Username == username);
        if (existingUser)
        {
            ModelState.AddModelError("", "Tên tài khoản đã tồn tại trên hệ thống.");
            return View();
        }

        var newUser = new User
        {
            Username = username,
            PasswordHash = SecurityHelper.HashPassword(password),
            FullName = fullName,
            Email = email,
            CompanyName = companyName,
            IsActive = true,
            RoleId = 3 // Default Role is 3 (Partner / Enterprise Account)
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đăng ký tài khoản doanh nghiệp thành công! Bạn có thể đăng nhập ngay bây giờ.";
        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    [Route("Auth/AccessDenied")]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToDashboard(string? roleName = null)
    {
        var role = roleName;
        if (string.IsNullOrEmpty(role))
        {
            if (User.IsInRole("SuperAdmin")) role = "SuperAdmin";
            else if (User.IsInRole("Editor")) role = "Editor";
            else if (User.IsInRole("Partner")) role = "Partner";
        }

        if (role == "SuperAdmin")
        {
            return RedirectToAction("Index", "Admin");
        }
        else if (role == "Editor")
        {
            return RedirectToAction("Posts", "Admin");
        }
        else
        {
            return RedirectToAction("Index", "Partner");
        }
    }
}
