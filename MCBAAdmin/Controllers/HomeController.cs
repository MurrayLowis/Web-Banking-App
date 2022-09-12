using Microsoft.AspNetCore.Mvc;
using SupportLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using X.PagedList;

namespace MCBAAdmin.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private const int PageSize = 4;
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public HomeController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;
    public IActionResult Index() => View();

    public IActionResult Privacy() => View();

    public IActionResult Error(string error)
    {
        // go to error page and display error message
        var errorView = new ErrorViewModel()
        {
            Error = error
        };
        return View(errorView);
    }

    public async Task<IActionResult> Summary(int page = 1)
    {
        var response = await Client.GetAsync($"api/customer/");

        // display error if nothing returned
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = "Connection errror"
            });

        // go to user customer summary page if successful
        var result = await response.Content.ReadAsStringAsync();
        List<CustomerViewModel> customers = JsonConvert.DeserializeObject<List<CustomerViewModel>>(result);
        // create paged list of customers
        var customerList = await customers.ToPagedListAsync(page, PageSize);
        return View("~/Views/Admin/Home.cshtml", customerList);
    }
}
