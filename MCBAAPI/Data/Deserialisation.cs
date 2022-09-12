using MCBAAPI.Models;
using SupportLibrary.ViewModels;
using SimpleHashing;
using MCBAAPI.Data;


namespace MCBAAPI.Data;

public static class Deserialisation
{
    public static Transaction DeserialiseTransaction(this TransactionViewModel data)
    {
        Transaction transaction = new();
        transaction.TransactionID = data.TransactionID;
        transaction.TransactionType = data.TransactionType;
        transaction.AccountNumber = data.AccountNumber;
        transaction.DestinationAccountNumber = data.DestinationAccountNumber;
        if (data.Amount != null)
            transaction.Amount = Convert.ToDecimal(data.Amount);
        if (data.Comment != null)
            transaction.Comment = data.Comment;
        transaction.TransactionTimeUtc = data.TransactionTimeUtc;
        return transaction;
    }

    public static TransactionViewModel SerialiseTransaction(this Transaction data)
    {
        TransactionViewModel transaction = new();
        transaction.TransactionID = data.TransactionID;
        transaction.TransactionType = data.TransactionType;
        transaction.AccountNumber = data.AccountNumber;
        transaction.DestinationAccountNumber = data.DestinationAccountNumber;
        transaction.Amount = data.Amount.ToString();
        if (data.Comment != null)
            transaction.Comment = data.Comment;
        transaction.TransactionTimeUtc = data.TransactionTimeUtc;
        return transaction;
    }

    public static Account DeserialiseAccount(this AccountViewModel data)
    {
        Account account = new();
        account.AccountNumber = data.AccountNumber;
        account.AccountType = data.AccountType;
        account.CustomerID = data.AccountNumber;
        account.Transactions = new();
        return account;
    }

    public static AccountViewModel SerialiseAccount(this Account data, DbAccess dbAccess)
    {
        AccountViewModel account = new();
        account.AccountNumber = data.AccountNumber;
        account.AccountType = data.AccountType;
        account.CustomerID = data.AccountNumber;
        account.Transactions = new();
        // read matching transactions from the database
        foreach (Transaction transaction in dbAccess.ReadAll(new Transaction()))
        {
            if (transaction.AccountNumber == data.AccountNumber)
                account.Transactions.Add(transaction.SerialiseTransaction());
        }
        return account;
    }

    public static Customer DeserialiseCustomer(this CustomerViewModel data)
    {
        Customer customer = new();
        customer.CustomerID = data.CustomerID;
        if (data.Name != null)
            customer.Name = data.Name;
        if (data.TFN != null)
            customer.TFN = data.TFN;
        if (data.Address != null)
            customer.Address = data.Address;
        if (data.City != null)
            customer.City = data.City;
        if (data.State != null)
            customer.State = data.State;
        if (data.PostCode != null)
            customer.PostCode = data.PostCode;
        if (data.Mobile != null)
            customer.Mobile = data.Mobile;
        customer.Frozen = data.Frozen;
        customer.Accounts = new();
        return customer;
    }

    public static CustomerViewModel SerialiseCustomer(this Customer data, DbAccess dbAccess)
    {
        CustomerViewModel customer = new();
        customer.CustomerID = data.CustomerID;
        if (data.Name != null)
            customer.Name = data.Name;
        if (data.TFN != null)
            customer.TFN = data.TFN;
        if (data.Address != null)
            customer.Address = data.Address;
        if (data.City != null)
            customer.City = data.City;
        if (data.State != null)
            customer.State = data.State;
        if (data.PostCode != null)
            customer.PostCode = data.PostCode;
        if (data.Mobile != null)
            customer.Mobile = data.Mobile;
        customer.Frozen = data.Frozen;
        customer.Accounts = new();
        // read matching accounts from the database
        foreach (Account account in dbAccess.ReadAll(new Account()))
        {
            if (account.CustomerID == data.CustomerID)
                customer.Accounts.Add(account.SerialiseAccount(dbAccess));
        }
        return customer;
    }

    public static Login DeserialiseLogin(this LoginViewModel data)
    {
        Login login = new();
        if (data.LoginID != null)
            login.LoginID = data.LoginID;
        login.CustomerID = data.CustomerID;
        // hash raw password input
        if (data.Password != null)
            login.PasswordHash = PBKDF2.Hash(data.Password, 50000);
        return login;
    }

    public static LoginViewModel SerialiseLogin(this Login data)
    {
        LoginViewModel login = new();
        if (data.LoginID != null)
            login.LoginID = data.LoginID;
        login.CustomerID = data.CustomerID;
        // do not return password or password hash
        return login;
    }

    public static BillPay DeserialiseBillPay(this BillPayViewModel data)
    {
        BillPay billPay = new();
        billPay.BillPayID = data.BillPayID;
        billPay.AccountNumber = data.AccountNumber;
        billPay.PayeeID = data.PayeeID;
        if (data.Amount != null)
            billPay.Amount = Convert.ToDecimal(data.Amount);
        billPay.ScheduleTimeUtc = data.ScheduleTimeUtc;
        billPay.Period = data.Period;
        billPay.PaymentsDue = data.PaymentsDue;
        billPay.Frozen = data.Frozen;
        billPay.Cancelled = data.Cancelled;
        return billPay;
    }

    public static BillPayViewModel SerialiseBillPay(this BillPay data)
    {
        BillPayViewModel billPay = new();
        billPay.BillPayID = data.BillPayID;
        billPay.AccountNumber = data.AccountNumber;
        billPay.PayeeID = data.PayeeID;
        billPay.Amount = data.Amount.ToString();
        billPay.ScheduleTimeUtc = data.ScheduleTimeUtc;
        billPay.Period = data.Period;
        billPay.PaymentsDue = data.PaymentsDue;
        billPay.Frozen = data.Frozen;
        billPay.Cancelled = data.Cancelled;
        return billPay;
    }

    public static Payee DeserialisePayee(this PayeeViewModel data)
    {
        Payee payee = new();
        payee.PayeeID = data.PayeeID;
        if (data.Name != null)
            payee.Name = data.Name;
        if (data.Address != null)
            payee.Address = data.Address;
        if (data.City != null)
            payee.City = data.City;
        if (data.State != null)
            payee.State = data.State;
        if (data.PostCode != null)
            payee.PostCode = data.PostCode;
        payee.Phone = data.Phone;
        return payee;
    }

    public static PayeeViewModel SerialisePayee(this Payee data)
    {
        PayeeViewModel payee = new();
        payee.PayeeID = data.PayeeID;
        if (data.Name != null)
            payee.Name = data.Name;
        if (data.Address != null)
            payee.Address = data.Address;
        if (data.City != null)
            payee.City = data.City;
        if (data.State != null)
            payee.State = data.State;
        if (data.PostCode != null)
            payee.PostCode = data.PostCode;
        payee.Phone = data.Phone;
        return payee;
    }
}
