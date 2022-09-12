using Microsoft.AspNetCore.Mvc;
using SupportLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MCBA.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Privacy() => View();

    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string error)
    {
        // go to error page and display error message
        var errorView = new ErrorViewModel()
        {
            Error = error
        };
        return View(errorView);
    }
}
