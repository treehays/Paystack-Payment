using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using testing.Models;

namespace testing.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITest _test;

    public HomeController(ILogger<HomeController> logger, ITest test)
    {
        _logger = logger;
        _test = test;
    }

    public async Task<IActionResult> Index(string accountNumber, string recipient, string bankCode, decimal amount)
    {
        //var SN = await _test.SendMoney(recipient, amount);
        //var VA = await _test.VerifyByAccountNumber(accountNumber, bankCode, amount);
        //var th = await _test.GenerateRecipients(VA);
        //var th = await _test.MakePayment(costOfProduct, productNumber, customerEmail);
        var th = await _test.MakePayment(3455, recipient, recipient);
        return View();
        //var th = await _test.VerifyAccountNumber(accountNumber, bankCode, amount);
        //return Redirect(th.data.authorization_url);
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
