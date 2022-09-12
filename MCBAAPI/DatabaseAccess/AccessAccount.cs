using MCBAAPI.Models;
using MCBAAPI.Data;

namespace MCBAAPI.DatabaseAccess
{
    public static class AccessAccount
    {
        public static void Create(McbaContext context, Account account)
        {
            context.Accounts.Add(account);
            var customer = context.Customers.Find(account.AccountNumber);
            customer.Accounts.Add(account);
        }

        public static Account Read(McbaContext context, int accountNumber)
        {
            var account = context.Accounts.Find(accountNumber);
            return account;
        }

        public static List<Account> ReadAll (McbaContext context)
        {
            var accountList = context.Accounts.ToList();
            return accountList;
        }

        public static void Update(McbaContext context, Account newAccount)
        {
            var oldAccount = context.Accounts.Find(newAccount.AccountNumber);
            if (oldAccount.AccountType != '\0')
                oldAccount.AccountType = newAccount.AccountType;
            if (oldAccount.CustomerID != 0)
                oldAccount.CustomerID = newAccount.CustomerID;
        }

        public static void Delete(McbaContext context, Account account)
        {
            // delete all associated transactions
            foreach (var transaction in account.Transactions)
            {
                AccessTransaction.Delete(context, transaction);
            }
            var customer = context.Customers.Find(account.AccountNumber);
            customer.Accounts.Remove(account);
        }
    }
}
