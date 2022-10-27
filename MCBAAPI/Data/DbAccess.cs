using MCBAAPI.Models;
using MCBAAPI.DatabaseAccess;
using MCBAAPI.Utilities;

namespace MCBAAPI.Data;

public interface IDbAccess
{
}

public class DbAccess : IDbAccess
{
    private readonly McbaContext _context;
    public DbAccess(McbaContext context) => _context = context;

    public string Create(IModel value)
    {
        string errorMessage = "";
        if (value is Transaction)
        {
            // only transactions are currently able to throw errors
            errorMessage = NewTransaction((Transaction)value);
        }
        else if (value is Account)
        {
            AccessAccount.Create(_context, (Account)value);
        }
        else if (value is Customer)
        {
            AccessCustomer.Create(_context, (Customer)value);
        }
        else if (value is Login)
        {
            AccessLogin.Create(_context, (Login)value);
        }
        else if (value is BillPay)
        {
            AccessBillPay.Create(_context, (BillPay)value);
        }
        else// if (value is Payee)
        {
            AccessPayee.Create(_context, (Payee)value);
        }

        if (errorMessage == "")
        {
            _context.SaveChanges();
            // have to return null (not an empty string) since an empty string is read as """" by HttpResponseMessage
            return null;
        }
        return errorMessage;
    }

    public IModel Read(IModel value)
    {
        if (value is Transaction)
        {
            value = AccessTransaction.Read(_context, ((Transaction)value).TransactionID);
        }
        else if (value is Account)
        {
            value = AccessAccount.Read(_context, ((Account)value).AccountNumber);
        }
        else if (value is Customer)
        {
            value = AccessCustomer.Read(_context, ((Customer)value).CustomerID);
        }
        else if (value is Login)
        {
            value = AccessLogin.Read(_context, ((Login)value).LoginID);
        }
        else if (value is BillPay)
        {
            value = AccessBillPay.Read(_context, ((BillPay)value).BillPayID);
        }
        else// if (value is Payee)
        {
            value = AccessPayee.Read(_context, ((Payee)value).PayeeID);
        }

        return value;
    }

    public List<IModel> ReadAll(IModel value)
    {
        var returnList = new List<IModel>();
        if (value is Transaction)
        {
            returnList.AddRange(AccessTransaction.ReadAll(_context));
        }
        else if (value is Account)
        {
            returnList.AddRange(AccessAccount.ReadAll(_context));
        }
        else if (value is Customer)
        {
            returnList.AddRange(AccessCustomer.ReadAll(_context));
        }
        else if (value is Login)
        {
            returnList.AddRange(AccessLogin.ReadAll(_context));
        }
        else if (value is BillPay)
        {
            returnList.AddRange(AccessBillPay.ReadAll(_context));
        }
        else// if (value is Payee)
        {
            returnList.AddRange(AccessPayee.ReadAll(_context));
        }

        return returnList;
    }

    public string Update(IModel value)
    {
        if (value is Transaction)
        {
            AccessTransaction.Update(_context, (Transaction)value);
        }
        else if (value is Account)
        {
            AccessAccount.Update(_context, (Account)value);
        }
        else if (value is Customer)
        {
            AccessCustomer.Update(_context, (Customer)value);
        }
        else if (value is Login)
        {
            AccessLogin.Update(_context, (Login)value);
        }
        else if (value is BillPay)
        {
            AccessBillPay.Update(_context, (BillPay)value);
        }
        else// if (value is Payee)
        {
            AccessPayee.Update(_context, (Payee)value);
        }

        if (_context.SaveChanges() == 0)
        {
            return "Conflict detected";
        }
        return null;
    }

    public string Delete(IModel value)
    {
        if (value is Transaction)
        {
            AccessTransaction.Delete(_context, (Transaction)value);
        }
        else if (value is Account)
        {
            AccessAccount.Delete(_context, (Account)value);
        }
        else if (value is Customer)
        {
            AccessCustomer.Delete(_context, (Customer)value);
        }
        else if (value is Login)
        {
            AccessLogin.Delete(_context, (Login)value);
        }
        else if (value is BillPay)
        {
            AccessBillPay.Delete(_context, (BillPay)value);
        }
        else// if (value is Payee)
        {
            AccessPayee.Delete(_context, (Payee)value);
        }

        if (_context.SaveChanges() == 0)
        {
            return "No match found";
        }
        return null;
    }

    private string NewTransaction(Transaction transaction)
    {
        string errorMessage = "";
        if (transaction.TransactionType == ID.DEPOSIT)
        {
            // no error for deposits
            AccessTransaction.Create(_context, transaction);
        }
        else
        {
            // attempt transfer, withdrawal, or bill pay
            errorMessage = AddOutgoing(transaction);
        }
        return errorMessage;
    }

    private string AddOutgoing(Transaction transaction)
    {
        // check destination account is valid for transfers
        string errorMessage = CheckTransferValidity(transaction);
        if (errorMessage.Equals(""))
        {
            var account = _context.Accounts.Find(transaction.AccountNumber);
            // get transactions from database
            account.Transactions = new();
            foreach (var accountTransaction in _context.Transactions)
            {
                if (accountTransaction.AccountNumber == transaction.AccountNumber)
                    account.Transactions.Add(accountTransaction);
            }
            // prepare a service charge transaction
            var serviceCharge = GetServiceCharge(transaction, account);

            decimal availableBalance = account.GetBalance();
            // confirm if balance is available
            availableBalance -= transaction.Amount;
            availableBalance -= serviceCharge.Amount;
            if (account.AccountType == ID.CHEQUE)
                availableBalance -= Value.CHEQUE_MIN;

            // only approve transaction once all checks passed
            if (availableBalance >= 0m)
            {
                // submit transaction to database
                AccessTransaction.Create(_context, transaction);
                // apply service charge if applicable
                if (serviceCharge.Amount != Value.VOID_TRANSACTION)
                {
                    AccessTransaction.Create(_context, serviceCharge);
                }
                // add incoming transaction to transfer recipient account
                AddIncomingTransfer(transaction);
            }
            // set error for insufficient balance
            else
                errorMessage = "Insufficient balance";
        }
        return errorMessage;
    }

    private string CheckTransferValidity(Transaction transaction)
    {
        string errorMessage = "";
        // check for correct destination account
        if (transaction.DestinationAccountNumber != null &&
            _context.Accounts.Find(transaction.DestinationAccountNumber) == null)
        {
            errorMessage = "Destination account does not exist";
        }
        else if (transaction.DestinationAccountNumber == transaction.AccountNumber)
        {
            errorMessage = "Account cannot send transfer to itself";
        }
        return errorMessage;
    }

    private Transaction GetServiceCharge(Transaction transaction, Account account)
    {
        Transaction serviceCharge = new();
        // service charge discarded if this is not overwritten
        serviceCharge.Amount = Value.VOID_TRANSACTION;
        // check if account has any free transactions remaining
        if (!account.HasFreeTransfersRemaining())
        {
            serviceCharge.TransactionType = ID.SERVICE_CHARGE;
            serviceCharge.AccountNumber = transaction.AccountNumber;
            if (transaction.TransactionType == ID.WITHDRAWAL)
            {
                serviceCharge.Amount = Value.WITHDRAW_CHARGE;
                serviceCharge.Comment = "Withdrawal fee";
            }
            else if (transaction.TransactionType == ID.TRANSFER)
            {
                serviceCharge.Amount = Value.TRANSFER_CHARGE;
                serviceCharge.Comment = "Transfer fee";
            }
        }
        return serviceCharge;
    }

    private void AddIncomingTransfer(Transaction transaction)
    {
        // add incoming transfer to recipient accounts if applicable
        if (transaction.DestinationAccountNumber != null)
        {
            Transaction incomingTransaction = new();
            incomingTransaction.TransactionType = transaction.TransactionType;
            incomingTransaction.AccountNumber = (int)transaction.DestinationAccountNumber;
            incomingTransaction.Amount = transaction.Amount;
            incomingTransaction.Comment = transaction.Comment;

            AccessTransaction.Create(_context, incomingTransaction);
        }
    }
}
