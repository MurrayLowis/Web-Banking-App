using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MCBAAPI.Models;

namespace MCBAAPI.Data;

public static class SeedData
{
    // URL for database seed
    private const string _seedUrl = "https://coreteaching03.csit.rmit.edu.au/~e103884/wdt/services/customers/";
    private static IConfigurationRoot _configuration { get; } = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    private static string _connectionString { get; } = _configuration["ConnectionString"];

    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<McbaContext>();

        // populate customer list from database
        if (!context.Customers.Any())
        {
            using var client = new HttpClient();
            var json = client.GetStringAsync(_seedUrl).Result;
            // since data types in seed data do not exactly match the model, just deserialise
            // into generic objects and then pass converted data into the actual model
            var jsonList = (JArray)JsonConvert.DeserializeObject(json);

            AddCustomers(jsonList, context);
            AddPayee(context);

            context.SaveChanges();
        }
    }

    // populate customers in McbaContext
    public static void AddCustomers(JArray customerList, McbaContext context)
    {
        foreach (JObject customer in customerList)
        {
            List<Account> accounts = AddAccounts((JArray)customer[nameof(Customer.Accounts)], context);
            // add new customer to McbaContext
            context.Customers.Add(
                new Customer
                {
                    CustomerID = (int)customer[nameof(Customer.CustomerID)],
                    Name = (string)customer[nameof(Customer.Name)],
                    Address = (string)customer[nameof(Customer.Address)],
                    City = (string)customer[nameof(Customer.City)],
                    PostCode = (string)customer[nameof(Customer.PostCode)],
                    Frozen = false,
                    Accounts = accounts
                });
            // add new login to McbaContext
            Login login = AddLogin((JObject)customer[nameof(Login)], context);
            login.CustomerID = (int)customer[nameof(Customer.CustomerID)];
            context.Logins.Add(login);
        }
    }

    // populate accounts in McbaContext and return account list to calling customer
    public static List<Account> AddAccounts(JArray accountList, McbaContext context)
    {
        // list to be returned
        List<Account> accounts = new();
        foreach (JObject account in accountList)
        {
            List<Transaction> transactions = AddTransactions((JArray)account[nameof(Account.Transactions)], context);
            Account newAccount = new Account
            {
                AccountNumber = (int)account[nameof(Account.AccountNumber)],
                AccountType = ((string)account[nameof(Account.AccountType)])[0],
                CustomerID = (int)account[nameof(Account.CustomerID)],
                Transactions = transactions
            };
            // add new account to McbaContext
            context.Accounts.Add(newAccount);
            // add account to list to be returned to calling customer
            accounts.Add(newAccount);
        }
        return accounts;
    }

    // populate transactions in McbaContext and return transaction list to calling account
    public static List<Transaction> AddTransactions(JArray transactionList, McbaContext context)
    {
        // list to be returned
        List<Transaction> transactions = new();
        foreach (JObject transaction in transactionList)
        {
            Transaction newTransaction = new Transaction
            {
                // all transactions in the seed data are deposits so this is hardcoded for now
                TransactionType = 'D',
                Amount = (int)transaction[nameof(Transaction.Amount)],
                Comment = (string)transaction[nameof(Transaction.Comment)],
                // it is assumed that the seed data was stored as a local time value
                TransactionTimeUtc = ((DateTime)transaction[nameof(Transaction.TransactionTimeUtc)]).ToUniversalTime()
            };
            // add new transaction to McbaContext
            context.Transactions.Add(newTransaction);
            // add transaction to list to be returned to calling account
            transactions.Add(newTransaction);
        }
        return transactions;
    }

    // return login to calling customer
    public static Login AddLogin(JObject login, McbaContext context)
    {
        Login newLogin = new Login
        {
            LoginID = (string)login[nameof(Login.LoginID)],
            PasswordHash = (string)login[nameof(Login.PasswordHash)]
        };
        return newLogin;
    }

    // populate hardcoded payees in McbaContext
    public static void AddPayee(McbaContext context)
    {
        Payee payee1 = new Payee
        {
            Name = "Greg the Payee",
            Address = "123 Payee's address",
            City = "Citytown",
            State = "VIC",
            PostCode = "1234",
            Phone = "(03) 1234 5678"
        };
        Payee payee2 = new Payee
        {
            Name = "Dergbert McBickleson",
            Address = "123 another place",
            City = "Towncity",
            State = "VIC",
            PostCode = "9876",
            Phone = "(03) 9876 5432"
        };
        context.Payees.Add(payee1);
        context.Payees.Add(payee2);
    }
}