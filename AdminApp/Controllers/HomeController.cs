using Microsoft.AspNetCore.Mvc;

namespace New_folder.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Root page of the Admin CMS App redirects directly to the Admin Dashboard
        return RedirectToAction("Index", "Admin");
    }

    public IActionResult Error()
    {
        return View();
    }
}
