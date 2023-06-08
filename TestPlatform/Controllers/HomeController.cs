using Microsoft.AspNetCore.Mvc;
using TestPlatform.Database;
using TestPlatform.Models;

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
        return View(_db.Tests);
    }
}