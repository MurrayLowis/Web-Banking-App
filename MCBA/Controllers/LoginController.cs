using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SupportLibrary.ViewModels;
using Newtonsoft.Json;

namespace MCBA.Controllers;

[AllowAnonymous]
[Route("/Mcba/SecureLogin")]
public class LoginController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public LoginController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public IActionResult Login()
    {
        var model = new LoginViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel data)
    {
        // request login object from database
        var loginResponse = await Client.GetAsync($"api/login/{data.LoginID}/{data.Password}");
        // display error if nothing returned
        if (!loginResponse.IsSuccessStatusCode)
        {
            ModelState.AddModelError("WrongCredentials", "Incorrect LoginID or Password");
            return View(data);
        }

        var loginViewModel = JsonConvert.DeserializeObject<LoginViewModel>(await loginResponse.Content.ReadAsStringAsync());
        // invalid credentials
        if (loginViewModel.CustomerID == 0)
        {
            ModelState.AddModelError("WrongCredentials", "Incorrect LoginID or Password");
            return View(data);
        }
        // request customer details from database
        var customerResponse = await Client.GetAsync($"api/customer/{loginViewModel.CustomerID}");
        if (!loginResponse.IsSuccessStatusCode)
        {
            ModelState.AddModelError("WrongCredentials", "Incorrect LoginID or Password");
            return View(data);
        }

        var customerViewModel = JsonConvert.DeserializeObject<CustomerViewModel>(await customerResponse.Content.ReadAsStringAsync());
        if (customerViewModel.Frozen == true)
        {
            ModelState.AddModelError("Frozen", "Account frozen. Get outta here!");
            return View(data);
        }

        // set session
        HttpContext.Session.SetString(nameof(CustomerViewModel.Name), customerViewModel.Name);
        HttpContext.Session.SetInt32(nameof(CustomerViewModel.CustomerID), customerViewModel.CustomerID);
        HttpContext.Session.SetString(nameof(LoginViewModel.LoginID), data.LoginID);
        return RedirectToAction("Summary", "Account");
    }

    [Route("LoggingOut")]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove(nameof(CustomerViewModel.Name));
        HttpContext.Session.Remove(nameof(CustomerViewModel.CustomerID));
        HttpContext.Session.Remove(nameof(LoginViewModel.LoginID));
        return RedirectToAction("Index", "Home");
    }
}
