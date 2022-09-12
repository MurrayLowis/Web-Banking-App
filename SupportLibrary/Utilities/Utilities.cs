using SupportLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SupportLibrary.Utilities;

public static class ID
{
    // transaction types
    public const char WITHDRAWAL = 'W';
    public const char DEPOSIT = 'D';
    public const char TRANSFER = 'T';
    public const char SERVICE_CHARGE = 'S';
    public const char BILL_PAY = 'B';

    // account types
    public const char SAVINGS = 'S';
    public const char CHEQUE = 'C';

    // BillPay periods
    public const char MONTHLY = 'M';
    public const char ONE_OFF = 'O';
    
    // Customer's account state
    public const char FROZEN = 'F';
    public const char NOT_FROZEN = 'N';
    
}

public static class Utilities
{
    // get account balance from Account
    public static decimal GetBalance(this AccountViewModel account) => account.Transactions.GetBalance();

    // get account balance from transaction list
    public static decimal GetBalance(this List<TransactionViewModel> transactions)
    {
        decimal balance = 0m;
        foreach (var transaction in transactions)
        {
            // increase balance for deposits and incoming transfers
            if (transaction.TransactionType == ID.DEPOSIT ||
                (transaction.TransactionType == ID.TRANSFER &&
                 transaction.DestinationAccountNumber == 0))
            {
                balance += Convert.ToDecimal(transaction.Amount);
            }
            // decrease balance for all other transactions
            else
            {
                balance -= Convert.ToDecimal(transaction.Amount);
            }
        }

        return balance;
    }

    // add model state errors to view model
    public static bool HasErrors(this ModelStateDictionary modelState)
    {
        bool errorsPresent = false;
        Dictionary<string, string> errors = new();
        // iterate through errors and add to list
        foreach (var state in modelState)
        {
            foreach (var error in state.Value.Errors)
            {
                if (!errors.ContainsKey(state.Key))
                {
                    errors.Add(state.Key, error.ErrorMessage);
                }
            }
        }

        // add list of errors to model state
        foreach (var error in errors)
        {
            errorsPresent = true;
            modelState.AddModelError(error.Key, error.Value);
        }

        return errorsPresent;
    }
}