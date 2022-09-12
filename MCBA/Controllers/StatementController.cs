using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using SupportLibrary.ViewModels;
using Newtonsoft.Json;

namespace MCBA.Controllers;

public class StatementController : Controller
{
    private const int PageSize = 4;
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public StatementController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    public async Task<IActionResult> StatementView(int page = 1)
    {
        // read account number from session
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
            OrderBy(x => x.TransactionTimeUtc).Reverse().
            ToPagedListAsync(page, PageSize);
        return View(transactionList);
    }
}