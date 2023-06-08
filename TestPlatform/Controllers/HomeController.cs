using Microsoft.AspNetCore.Mvc;
using TestPlatform.Database;

namespace TestPlatform.Controllers;

public class HomeController : Controller
{
    private readonly TestPlatformContext _db;

    public HomeController(TestPlatformContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var test = _db.Tests;
        return View(test);
    }
}