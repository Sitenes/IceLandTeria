using Microsoft.AspNetCore.Mvc;

namespace IceLandTeria.Controllers;

[Route("Admin")]
public class AdminController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}