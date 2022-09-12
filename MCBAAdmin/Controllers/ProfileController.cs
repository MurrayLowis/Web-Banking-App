using Microsoft.AspNetCore.Mvc;
using SupportLibrary.Utilities;
using SupportLibrary.ViewModels;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;

namespace MCBAAdmin.Controllers;

public class ProfileController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public ProfileController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    public async Task<IActionResult> ProfileView(int customerID)
    {
        var response = await Client.GetAsync($"api/customer/{customerID}");

        // display error if nothing returned
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = "Failed to contact the database"
            });

        // go to user profile view page if successful
        var result = await response.Content.ReadAsStringAsync();
        CustomerViewModel customerViewModel = JsonConvert.DeserializeObject<CustomerViewModel>(result);
        return View(customerViewModel);
    }

    public IActionResult ProfileEdit(CustomerViewModel data)
    {
        if (ModelState.HasErrors())
        {
            return View(data);
        }
        return View(data);
    }

    [HttpPost]
    public IActionResult ProfileEditSubmit(
        [Bind("CustomerID,Name,TFN,Address,City,State,PostCode,Mobile")]
        CustomerViewModel data)
    {
        // if any errors, return to previous page and show errors
        if (ModelState.HasErrors())
            return View("~/Views/Profile/ProfileEdit.cshtml", data);

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
        Debug.WriteLine("CID = " + data.CustomerID);
        return View("~/Views/Profile/ProfileView.cshtml", data);
    }
}