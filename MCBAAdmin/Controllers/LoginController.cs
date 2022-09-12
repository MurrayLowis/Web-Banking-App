using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SupportLibrary.ViewModels;

namespace MCBAAdmin.Controllers;

[AllowAnonymous]
[Route("/Mcba/SecureLogin")]
public class LoginController : Controller
{
    [HttpPost]
    public IActionResult Login(LoginViewModel data)
    {
        if (!data.LoginID.Equals("admin") || !data.Password.Equals("admin"))
        {
            ModelState.AddModelError("WrongCredentials", "Incorrect LoginID or Password");
            return RedirectToAction("Index", "Home");
        }

        HttpContext.Session.SetInt32("LoggedIn", 1);
        return RedirectToAction("Summary", "Home");
    }

    [Route("LoggingOut")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        ;
        return RedirectToAction("Index", "Home");
    }
}