using Microsoft.AspNetCore.Mvc;
using SupportLibrary.ViewModels;
using Newtonsoft.Json;
using System.Text;

namespace MCBAAdmin.Controllers;
public class AccountController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public AccountController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    public async Task<IActionResult> Summary(int customerID)
    {
        var response = await Client.GetAsync($"api/customer/{customerID}");

        // display error if nothing returned
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = "No match found"
            });

        // set customerID to session
        HttpContext.Session.SetInt32(nameof(CustomerViewModel.CustomerID), customerID);

        // go to user accounts summary page if successful
        var result = await response.Content.ReadAsStringAsync();
        CustomerViewModel customerViewModel = JsonConvert.DeserializeObject<CustomerViewModel>(result);
        return View(customerViewModel);
    }

    public async Task<IActionResult> Options(int accountNumber)
    {
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

    [HttpPost]
    public IActionResult Freeze(
        [Bind("CustomerID,Name,TFN,Address,City,State,PostCode,Mobile,Frozen")]
        CustomerViewModel data)
    {
        // toggle frozen status
        data.Frozen = !data.Frozen;

        // send update request to database with updated customer details
        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

        var response = Client.PutAsync("api/customer", content).Result;

        // if error is returned, display specific error message
        if (!response.Content.ReadAsStringAsync().Result.Equals(""))
            return RedirectToAction("Error", "Home", new
            {
                error = response.Content.ReadAsStringAsync().Result
            });
        // return to profile page if successful
        return RedirectToAction("Summary", "Account", data);
    }
}