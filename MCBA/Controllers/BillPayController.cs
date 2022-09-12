using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
using SupportLibrary.Utilities;
using SupportLibrary.ViewModels;
using System.Text;
using System.Globalization;

namespace MCBA.Controllers;

public class BillPayController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");
    private const int PageSize = 4;

    public BillPayController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

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
        // go straight to new billPay if no active bill pays exist
        if (billPayList.Count() == 0)
        {
            var model = new BillPayViewModel();
            model.Payees = new SelectList(payeeNames);
            model.AccountNumber = accountNumber;
            return View("~/Views/BillPay/BillPay.cshtml", model);
        }
        return View(billPayList);
    }

    public async Task<IActionResult> BillPay(int accountNumber)
    {
        var payees = await GetPayeeList();
        List<string> payeeNames = GetPayeeNames(payees);
        var model = new BillPayViewModel();
        model.Payees = new SelectList(payeeNames);
        model.AccountNumber = accountNumber;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> BillPay(BillPayViewModel data)
    {
        // add error for decimal overflow
        if (data.Amount != null && data.Amount.Length > 15)
            ModelState.AddModelError("Amount", "Stop trying to crash our database. 20 characters or fewer.");
        // return to input screen with error messages
        if (ModelState.HasErrors())
        {
            var payees = await GetPayeeList();
            List<string> payeeNames = GetPayeeNames(payees);
            data.Payees = new SelectList(payeeNames);
            return View(data);
        }
        // proceed to confirmation if no errors
        return View("~/Views/BillPay/BillPayConfirmation.cshtml", data);
    }

    [HttpPost]
    public async Task<IActionResult> BillPaySubmit(BillPayViewModel data, string time)
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
        // set payee ID from payee name
        var payees = await GetPayeeList();
        foreach (var payee in payees)
        {
            if (data.PayeeName.Equals(payee.Name))
                data.PayeeID = payee.PayeeID;
        }
        // submit to database
        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await Client.PostAsync("api/billpay", content);
        // if error is returned, display specific error message
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = response.Content.ReadAsStringAsync().Result
            });
        // return to accounts page if successful
        return RedirectToAction("Summary", "Account");
    }

    public async Task<IActionResult> BillPayEdit(int billPayId, int payeeID)
    {
        var payees = await GetPayeeList();
        List<string> payeeNames = GetPayeeNames(payees);

        // request relevant bill pay from database
        var response = await Client.GetAsync($"api/billpay/{billPayId}");
        // if error is returned, display specific error message
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = response.Content.ReadAsStringAsync().Result
            });

        // proceed to edit page if successful
        var result = await response.Content.ReadAsStringAsync();
        BillPayViewModel loginViewModel = JsonConvert.DeserializeObject<BillPayViewModel>(result);

        // set payee name for billpay
        foreach (var payee in payees)
            if (payee.PayeeID == payeeID)
                loginViewModel.PayeeName = payee.Name;
        // add payees to selection list
        loginViewModel.Payees = new SelectList(payeeNames);
        return View(loginViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> BillPayEdit(BillPayViewModel data)
    {
        var payees = await GetPayeeList();
        List<string> payeeNames = GetPayeeNames(payees);

        // add error for decimal overflow
        if (data.Amount != null && data.Amount.Length > 15)
            ModelState.AddModelError("Amount", "Stop trying to crash our database. 20 characters or fewer.");
        // return to input screen with error messages
        if (ModelState.HasErrors())
        {
            data.Payees = new SelectList(payeeNames);
            return View(data);
        }

        // proceed to confirmation if no errors
        return View("~/Views/BillPay/BillPayEditConfirmation.cshtml", data);
    }

    [HttpPost]
    public async Task<IActionResult> BillPayEditSubmit(BillPayViewModel data, string time)
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
        // set payee ID from payee name
        var payees = await GetPayeeList();
        foreach (var payee in payees)
        {
            if (data.PayeeName.Equals(payee.Name))
                data.PayeeID = payee.PayeeID;
        }

        // submit to database
        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await Client.PutAsync("api/billpay", content);
        // if error is returned, display specific error message
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = response.Content.ReadAsStringAsync().Result
            });
        // return to accounts page if successful
        return RedirectToAction("Summary", "Account");
    }

    public async Task<IActionResult> BillPayCancel(BillPayViewModel data, string time)
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
        // set to cancelled
        data.Cancelled = true;
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
        return RedirectToAction("Summary", "Account");
    }

    public async Task<IActionResult> PayOutstanding(BillPayViewModel data, string time)
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
        // attempt to submit a new bill pay transaction
        var transaction = new TransactionViewModel();
        transaction.TransactionType = ID.BILL_PAY;
        transaction.AccountNumber = data.AccountNumber;
        transaction.Amount = Convert.ToDecimal(data.Amount).ToString("n2");
        transaction.Comment = "Manual BillPay payment";

        // submit to database
        var transactionContent = new StringContent(JsonConvert.SerializeObject(transaction), Encoding.UTF8, "application/json");
        var response = await Client.PostAsync("api/transaction", transactionContent);

        // if error is returned, display specific error message
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = response.Content.ReadAsStringAsync().Result
            });

        // TODO figure out some way to ensure bill pay is udpated even if an error occurs after this point
        // decrement payments due
        data.PaymentsDue = -1;
        // set payee name
        var payees = await GetPayeeList();
        foreach (var payee in payees)
        {
            if (data.PayeeID.Equals(payee.PayeeID))
                data.PayeeName = payee.Name;
        }
        data.Amount = Convert.ToDecimal(data.Amount).ToString("n2");
        var billPayContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var submitResponse = await Client.PutAsync("api/billpay", billPayContent);

        // if error is returned, display specific error message
        if (!submitResponse.IsSuccessStatusCode)
            return RedirectToAction("Error", "Home", new
            {
                error = submitResponse.Content.ReadAsStringAsync().Result
            });
        // return to accounts page if successful
        return RedirectToAction("Summary", "Account");
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