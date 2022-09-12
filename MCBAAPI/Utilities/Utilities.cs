using MCBAAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MCBAAPI.Utilities;

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
}

public static class Value
{
    // account balance minimum
    public const decimal CHEQUE_MIN = 300m;
    public const decimal SAVINGS_MIN = 0m;

    // void transaction if this value is encountered
    public const decimal VOID_TRANSACTION = 0m;

    // service charges
    public const decimal WITHDRAW_CHARGE = 0.05m;
    public const decimal TRANSFER_CHARGE = 0.1m;
}

public static class Utilities
{
    // get account balance from Account
    public static decimal GetBalance(this Account account) => account.Transactions.GetBalance();

    // get account balance from transaction list
    public static decimal GetBalance(this List<Transaction> transactions)
    {
        decimal balance = 0m;
        foreach (var transaction in transactions)
        {
            // increase balance for deposits and incoming transfers
            if (transaction.TransactionType == ID.DEPOSIT ||
                (transaction.TransactionType == ID.TRANSFER &&
                 transaction.DestinationAccountNumber == 0))
            {
                balance += transaction.Amount;
            }
            // decrease balance for all other transactions
            else
            {
                balance -= transaction.Amount;
            }
        }

        return balance;
    }

    // check whether account has any free transfers remaining
    public static bool HasFreeTransfersRemaining(this Account account)
    {
        bool freeTransfersRemaining = true;
        int startingFreeTransactions = 2;
        foreach (var transaction in account.Transactions)
        {
            // only count outgoing transfers and withdrawals
            if (transaction.TransactionType == ID.WITHDRAWAL ||
                (transaction.TransactionType == ID.TRANSFER &&
                 transaction.AccountNumber != transaction.DestinationAccountNumber))
            {
                startingFreeTransactions--;
            }
        }

        if (startingFreeTransactions <= 0)
        {
            freeTransfersRemaining = false;
        }

        return freeTransfersRemaining;
    }
}