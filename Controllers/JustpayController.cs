using Microsoft.AspNetCore.Mvc;

namespace testing.Controllers;
public class JustpayController : Controller
{
    public async Task<IActionResult> Index()
    {

        return View();
    }

    public IActionResult Payment()
    {

        return View();
    }
}