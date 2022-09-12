using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using SupportLibrary.ViewModels;
using Newtonsoft.Json;
using System.Globalization;

namespace MCBAAdmin.Controllers;

public class StatementController : Controller
{
    private const int PageSize = 4;
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public StatementController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    public async Task<IActionResult> StatementView(int page = 1)
    {
        // possible incoming formats
        string[] dateFormats = new[] {
            "dd/MM/yyyy hh:mm:ss tt",
            "dd/MM/yyyy h:mm:ss tt",
            "d/MM/yyyy hh:mm:ss tt",
            "d/MM/yyyy h:mm:ss tt"
        };
        CultureInfo provider = new CultureInfo("en-US");
        // read parameters from session
        var accountNumber = HttpContext.Session.GetInt32(nameof(AccountViewModel.AccountNumber));
        var startString = HttpContext.Session.GetString("startDate");
        var endString = HttpContext.Session.GetString("endDate");

        // default dates
        DateTime start = DateTime.MinValue;
        DateTime end = DateTime.MaxValue;
        // if dates set by user, use that instead
        if (startString != null)
            start = DateTime.ParseExact(startString, dateFormats, provider).ToUniversalTime();
        if (endString != null)
            end = DateTime.ParseExact(endString, dateFormats, provider).ToUniversalTime();

        var response = await Client.GetAsync($"api/account/{accountNumber}");
        // display error if nothing returned
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = "Failed to contact the database"
            });

        // go to statement page if successful
        var result = await response.Content.ReadAsStringAsync();
        var accountViewModel = JsonConvert.DeserializeObject<AccountViewModel>(result);
        // filter transactions
        var transactionList = await accountViewModel.Transactions.
            // filter out range
            Where(x => x.TransactionTimeUtc > start && x.TransactionTimeUtc < end).
            OrderBy(x => x.TransactionTimeUtc).Reverse().
            ToPagedListAsync(page, PageSize);
        return View(transactionList);
    }

    public async Task<IActionResult> FilterDates(DateTime start, DateTime end)
    {
        // set to default value if not provided
        if (start == DateTime.MinValue)
            start = DateTime.MinValue;
        if (end == DateTime.MinValue)
            end = DateTime.MaxValue;
        // set date range to session
        HttpContext.Session.SetString("startDate", start.ToString());
        HttpContext.Session.SetString("endDate", end.ToString());

        // read parameters from session
        var accountNumber = HttpContext.Session.GetInt32(nameof(AccountViewModel.AccountNumber));
        var response = await Client.GetAsync($"api/account/{accountNumber}");
        // display error if nothing returned
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = "Failed to contact the database"
            });
        // go to statement page if successful
        var result = await response.Content.ReadAsStringAsync();
        var accountViewModel = JsonConvert.DeserializeObject<AccountViewModel>(result);
        // filter transactions
        var transactionList = await accountViewModel.Transactions.
            // filter out range
            Where(x => x.TransactionTimeUtc > start && x.TransactionTimeUtc < end).
            OrderBy(x => x.TransactionTimeUtc).Reverse().
            ToPagedListAsync(1, PageSize);
        return View("~/Views/Statement/StatementView.cshtml", transactionList);
    }

    public async Task<IActionResult> ResetFilter()
    {
        DateTime start = new DateTime(2010, 1, 1);
        DateTime end = new DateTime(2030, 1, 1);
        HttpContext.Session.SetString("startDate", start.ToString());
        HttpContext.Session.SetString("endDate", end.ToString());

        // read parameters from session
        var accountNumber = HttpContext.Session.GetInt32(nameof(AccountViewModel.AccountNumber));
        var response = await Client.GetAsync($"api/account/{accountNumber}");
        // display error if nothing returned
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = "Failed to contact the database"
            });
        // go to statement page if successful
        var result = await response.Content.ReadAsStringAsync();
        var accountViewModel = JsonConvert.DeserializeObject<AccountViewModel>(result);
        // filter transactions
        var transactionList = await accountViewModel.Transactions.
            // filter out range
            Where(x => x.TransactionTimeUtc > start && x.TransactionTimeUtc < end).
            OrderBy(x => x.TransactionTimeUtc).Reverse().
            ToPagedListAsync(1, PageSize);
        return View("~/Views/Statement/StatementView.cshtml", transactionList);
    }
}