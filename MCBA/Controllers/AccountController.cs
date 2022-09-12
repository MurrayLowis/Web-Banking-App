using Microsoft.AspNetCore.Mvc;
using SupportLibrary.ViewModels;
using Newtonsoft.Json;

namespace MCBA.Controllers;
public class AccountController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public AccountController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    public async Task<IActionResult> Summary()
    {
        // use CustomerID stored in session data to request Customer from database
        var customerID = HttpContext.Session.GetInt32(nameof(LoginViewModel.CustomerID));
        var response = await Client.GetAsync($"api/customer/{customerID}");

        // display error if nothing returned
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = "No match found"
            });

        // go to user accounts summary page if successful
        var result = await response.Content.ReadAsStringAsync();
        CustomerViewModel customerViewModel = JsonConvert.DeserializeObject<CustomerViewModel>(result);
        return View(customerViewModel);
    }

    public async Task<IActionResult> Options(int accountNumber)
    {
        // set account number to session
        HttpContext.Session.SetInt32(nameof(AccountViewModel.AccountNumber), accountNumber);
        // request account from database
        var response = await Client.GetAsync($"api/account/{accountNumber}");

        // display error if nothing returned
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = "Failed to contact the database"
            });
        // display error message if returned value is empty
        if (response.Content == null)
            return RedirectToAction("Error", "Home", new
            {
                error = "No match found"
            });

        // go to user account page if successful
        var result = await response.Content.ReadAsStringAsync();
        AccountViewModel accountViewModel = JsonConvert.DeserializeObject<AccountViewModel>(result);
        // set account number to session
        HttpContext.Session.SetInt32(nameof(AccountViewModel.AccountNumber), accountNumber);
        return View(accountViewModel);
    }
}