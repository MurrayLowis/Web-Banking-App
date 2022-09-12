using MCBAAPI.Data;
using MCBAAPI.Models;
using MCBAAPI.Utilities;

namespace MCBAAPI.Background
{
    public class BillPayService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BillPayService(IServiceProvider services,
            IServiceScopeFactory serviceScopeFactory)
        {
            _services = services;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await CheckBillPays(cancellationToken);

                // attempt operation every 1 minute
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        private async Task CheckBillPays(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<McbaContext>();

            var billPays = context.BillPays.ToList();
            foreach (var billPay in billPays)
            {
                // lodge BillPay if past scheduled time, payments are outstanding, and BillPay is active
                if (billPay.ScheduleTimeUtc < DateTime.UtcNow &&
                    billPay.PaymentsDue > 0 &&
                    !billPay.Frozen &&
                    !billPay.Cancelled)
                {
                    LodgeBillPay(context, billPay);
                }
            }
            await context.SaveChangesAsync(cancellationToken);
        }

        public void LodgeBillPay(McbaContext context, BillPay billPay)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbAccess = scope.ServiceProvider.GetRequiredService<DbAccess>();
            var account = context.Accounts.Find(billPay.AccountNumber);
            account.Transactions = new();
            // get account transactions
            List<IModel> transactions = dbAccess.ReadAll(new Transaction());
            foreach (Transaction transaction in transactions)
            {
                if (transaction.AccountNumber == account.AccountNumber)
                    account.Transactions.Add(transaction);
            }

            // update date for next scheduled payment for recurring payments
            if (billPay.Period == ID.MONTHLY)
            {
                // schedule next payment
                billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.AddMonths(1);
                billPay.PaymentsDue += 1;
            }
            // check for sufficient balance in account
            if (account.GetBalance() > billPay.Amount)
            {
                // add transaction to database
                BillPayTransaction(dbAccess, billPay);
                // reduce paymentsDue
                billPay.PaymentsDue -= 1;
            }
            // send update to database
            dbAccess.Update(billPay);
        }

        // create new transaction from billPay payment
        public void BillPayTransaction(DbAccess dbAccess, BillPay billPay)
        {
            var transaction = new Transaction();
            transaction.TransactionType = ID.BILL_PAY;
            transaction.AccountNumber = billPay.AccountNumber;
            transaction.Amount = billPay.Amount;
            // get payee name
            List<IModel> payees = dbAccess.ReadAll(new Payee());
            foreach (Payee payee in payees)
            {
                if (billPay.PayeeID == payee.PayeeID)
                    // limit of 30 characters
                    transaction.Comment = ("BillPay payment to " + payee.Name).Substring(0, 30);
            }
            // send transaction to database with billPay payment details
            dbAccess.Create(transaction);
        }
    }
}
