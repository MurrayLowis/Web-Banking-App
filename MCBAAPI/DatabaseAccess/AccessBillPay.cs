using MCBAAPI.Models;
using MCBAAPI.Data;

namespace MCBAAPI.DatabaseAccess
{
    public static class AccessBillPay
    {
        public static void Create(McbaContext context, BillPay billPay)
        {
            context.BillPays.Add(billPay);
        }

        public static BillPay Read(McbaContext context, int billPayId)
        {
            var billPay = context.BillPays.Find(billPayId);
            return billPay;
        }

        public static List<BillPay> ReadAll(McbaContext context)
        {
            var billPayList = context.BillPays.ToList();
            return billPayList;
        }

        public static void Update(McbaContext context, BillPay newBillPay)
        {
            var oldBillPay = context.BillPays.Find(newBillPay.BillPayID);
            if (oldBillPay.BillPayID != 0)
                oldBillPay.BillPayID = newBillPay.BillPayID;
            if (oldBillPay.AccountNumber != 0)
                oldBillPay.AccountNumber = newBillPay.AccountNumber;
            if (oldBillPay.PayeeID != 0)
                oldBillPay.PayeeID = newBillPay.PayeeID;
            if (oldBillPay.Amount != 0)
                oldBillPay.Amount = newBillPay.Amount;
            if (oldBillPay.ScheduleTimeUtc != DateTime.MinValue)
                oldBillPay.ScheduleTimeUtc = newBillPay.ScheduleTimeUtc;
            if (oldBillPay.Period != '\0')
                oldBillPay.Period = newBillPay.Period;
            // only action possible is to decrement by 1
            if (newBillPay.PaymentsDue < oldBillPay.PaymentsDue &&
                oldBillPay.PaymentsDue > 0)
                oldBillPay.PaymentsDue--;
            oldBillPay.Frozen = newBillPay.Frozen;
            oldBillPay.Cancelled = newBillPay.Cancelled;
        }

        public static void Delete(McbaContext context, BillPay billPay)
        {
            context.BillPays.Remove(billPay);
        }
    }
}
