using Microsoft.AspNetCore.Mvc;
using SupportLibrary.ViewModels;
using SupportLibrary.Utilities;
using Newtonsoft.Json;
using System.Text;

namespace MCBA.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");

        public TransactionController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        // go to transaction menu for deposits
        public IActionResult Deposit(int accountNumber)
        {
            var model = new TransactionViewModel();
            model.TransactionType = ID.DEPOSIT;
            model.AccountNumber = accountNumber;
            return View("~/Views/Transaction/Transaction.cshtml", model);
        }

        // go to transaction menu for withdrawals
        public IActionResult Withdrawal(int accountNumber)
        {
            var model = new TransactionViewModel();
            model.TransactionType = ID.WITHDRAWAL;
            model.AccountNumber = accountNumber;
            return View("~/Views/Transaction/Transaction.cshtml", model);
        }

        // go to transaction menu for transfers
        public IActionResult Transfer(int accountNumber)
        {
            var model = new TransactionViewModel();
            model.TransactionType = ID.TRANSFER;
            model.AccountNumber = accountNumber;
            return View("~/Views/Transaction/Transaction.cshtml", model);
        }

        [HttpPost]
        public IActionResult Transaction(TransactionViewModel data)
        {
            // add error for decimal overflow
            if (data.Amount != null && data.Amount.Length > 15)
                ModelState.AddModelError("Amount", "Stop trying to crash our database. 20 characters or fewer.");
            // add error for missing destination account for transfers
            if (data.TransactionType == ID.TRANSFER && !data.DestinationAccountNumber.HasValue)
                ModelState.AddModelError("DestinationAccountNumber", "Destination account required");
            // add remaining errors and return to input screen with error messages if any present
            if (ModelState.HasErrors())
                return View(data);

            // proceed to confirmation if no errors
            return View("~/Views/Transaction/TransactionConfirmation.cshtml", data);
        }

        [HttpPost]
        public IActionResult TransactionSubmit(TransactionViewModel data)
        {
            // send create request to database with updated new transaction details
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = Client.PostAsync("api/transaction", content).Result;

            // if error is returned, display specific error message
            if (!response.Content.ReadAsStringAsync().Result.Equals(""))
                return RedirectToAction("Error", "Home", new
                {
                    error = response.Content.ReadAsStringAsync().Result
                });
            // proceed if no errors
            return RedirectToAction("Summary", "Account");
        }
    }
}