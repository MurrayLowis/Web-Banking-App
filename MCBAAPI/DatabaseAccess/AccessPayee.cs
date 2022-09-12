using MCBAAPI.Models;
using MCBAAPI.Data;

namespace MCBAAPI.DatabaseAccess
{
    public static class AccessPayee
    {
        public static void Create(McbaContext context, Payee payee)
        {
            context.Payees.Add(payee);
        }

        public static Payee Read(McbaContext context, int payeeId)
        {
            var payee = context.Payees.Find(payeeId);
            return payee;
        }

        public static List<Payee> ReadAll(McbaContext context)
        {
            var payeeList = context.Payees.ToList();
            return payeeList;
        }

        public static void Update(McbaContext context, Payee newPayee)
        {
            var oldPayee = context.Payees.Find(newPayee.PayeeID);
            if(newPayee.PayeeID != 0)
                oldPayee.PayeeID = newPayee.PayeeID;
            if (newPayee.Name != null)
                oldPayee.Name = newPayee.Name;
            if (newPayee.Address != null)
                oldPayee.Address = newPayee.Address;
            if (newPayee.City != null)
                oldPayee.City = newPayee.City;
            if (newPayee.State != null)
                oldPayee.State = newPayee.State;
            if (newPayee.PostCode != null)
                oldPayee.PostCode = newPayee.PostCode;
            if (newPayee.Phone != null)
                oldPayee.Phone = newPayee.Phone;
        }

        public static void Delete(McbaContext context, Payee payee)
        {
            context.Payees.Remove(payee);
        }
    }
}
