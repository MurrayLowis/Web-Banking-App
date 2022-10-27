using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;
using SupportLibrary.ViewModels;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace MCBAAdmin.Controllers;

public class BillPayController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");
    private const int PageSize = 4;

    public BillPayController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    // open bill pays summary, start on page 1
    public async Task<IActionResult> BillPaySummary(int page = 1)
    {
        // read account number from session
        var accountNumber = (int)HttpContext.Session.GetInt32(nameof(AccountViewModel.AccountNumber));
        var payees = await GetPayeeList();
        List<string> payeeNames = GetPayeeNames(payees);

        // retrieve all active bill pays
        var billPayResponse = await Client.GetAsync("api/billpay");
        // display error if nothing returned
        if (!billPayResponse.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = "Failed to contact the database"
            });

        var billPayResult = await billPayResponse.Content.ReadAsStringAsync();
        List<BillPayViewModel> billPays = JsonConvert.DeserializeObject<List<BillPayViewModel>>(billPayResult);
        // set payee name for each billpay
        foreach (var billPay in billPays)
            foreach (var payee in payees)
                if (payee.PayeeID == billPay.PayeeID)
                    billPay.PayeeName = payee.Name;

        // filter only the customer's bill pays
        var billPayList = billPays.Where(x => x.AccountNumber == accountNumber).ToPagedList(page, PageSize);
        return View(billPayList);
    }

    public async Task<IActionResult> Freeze(BillPayViewModel data, string time)
    {
        // possible incoming formats
        string[] dateFormats = new[] {
            "dd/MM/yyyy hh:mm:ss tt",
            "dd/MM/yyyy h:mm:ss tt",
            "d/MM/yyyy hh:mm:ss tt",
            "d/MM/yyyy h:mm:ss tt"
        };
        CultureInfo provider = new CultureInfo("en-US");
        data.ScheduleTimeUtc = DateTime.ParseExact(time, dateFormats, provider).ToUniversalTime();
        // toggle frozen status
        data.Frozen = !data.Frozen;
        // reformat amount from string
        data.Amount = Convert.ToDecimal(data.Amount).ToString("n2");
        // set payee name
        var payees = await GetPayeeList();
        foreach (var payee in payees)
        {
            if (data.PayeeID.Equals(payee.PayeeID))
                data.PayeeName = payee.Name;
        }

        // submit to database
        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var submitResponse = await Client.PutAsync("api/billpay", content);

        // if error is returned, display specific error message
        if (!submitResponse.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = submitResponse.Content.ReadAsStringAsync().Result
            });
        // return to accounts page if successful
        return RedirectToAction("BillPaySummary", "BillPay", data.AccountNumber);
    }

    // request list of payees from database
    public async Task<List<PayeeViewModel>> GetPayeeList()
    {
        using var response = await Client.GetAsync($"api/payee");
        var payeeResult = await response.Content.ReadAsStringAsync();
        var payees = JsonConvert.DeserializeObject<List<PayeeViewModel>>(payeeResult);
        return payees;
    }

    // get list of payee names
    public List<string> GetPayeeNames(List<PayeeViewModel> payees)
    {
        List<string> payeeNames = new();
        foreach (var payee in payees)
            payeeNames.Add(payee.Name);
        return payeeNames;
    }
}