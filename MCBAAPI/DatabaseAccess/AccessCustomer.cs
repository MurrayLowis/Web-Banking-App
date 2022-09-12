using MCBAAPI.Models;
using MCBAAPI.Data;

namespace MCBAAPI.DatabaseAccess
{
    public static class AccessCustomer
    {
        public static void Create(McbaContext context, Customer customer)
        {
            context.Customers.Add(customer);
            var login = new Login
            {
                // TODO how are we assigning LoginIDs?
                LoginID = "",
                CustomerID = customer.CustomerID,
                // TODO how are we creating PasswordHashes?
                PasswordHash = ""
            };
            AccessLogin.Create(context, login);
        }

        public static Customer Read(McbaContext context, int customerId)
        {
            var customer = context.Customers.Find(customerId);
            return customer;
        }

        public static List<Customer> ReadAll(McbaContext context)
        {
            var customerList = context.Customers.ToList();
            return customerList;
        }

        public static void Update(McbaContext context, Customer newCustomer)
        {
            var oldCustomer = context.Customers.Find(newCustomer.CustomerID);
            if (newCustomer.Name != null)
                oldCustomer.Name = newCustomer.Name;
            oldCustomer.TFN = newCustomer.TFN;
            oldCustomer.Address = newCustomer.Address;
            oldCustomer.City = newCustomer.City;
            oldCustomer.State = newCustomer.State;
            oldCustomer.PostCode = newCustomer.PostCode;
            oldCustomer.Mobile = newCustomer.Mobile;
            // pass this value every time
            oldCustomer.Frozen = newCustomer.Frozen;
        }

        public static void Delete(McbaContext context, Customer customer)
        {
            // delete all associated accounts
            foreach (var account in customer.Accounts)
            {
                AccessAccount.Delete(context, account);
            }
            context.Customers.Remove(customer);
            // TODO delete associated login
        }
    }
}
