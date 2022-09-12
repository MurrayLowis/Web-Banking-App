using MCBAAPI.Models;
using MCBAAPI.Data;

namespace MCBAAPI.DatabaseAccess
{
    public static class AccessTransaction
    {
        public static void Create(McbaContext context, Transaction transaction)
        {
            transaction.TransactionTimeUtc = DateTime.UtcNow;
            context.Transactions.Add(transaction);
            var account = context.Accounts.Find(transaction.AccountNumber);
            account.Transactions.Add(transaction);
        }

        public static Transaction Read(McbaContext context, int transactionId)
        {
            var transaction = context.Transactions.Find(transactionId);
            return transaction;
        }

        public static List<Transaction> ReadAll(McbaContext context)
        {
            var transactionList = context.Transactions.ToList();
            return transactionList;
        }

        public static void Update(McbaContext context, Transaction newTransaction)
        {
            var oldTransaction = context.Transactions.Find(newTransaction.TransactionID);
            if(newTransaction.TransactionID != 0)
                oldTransaction.TransactionID = newTransaction.TransactionID;
            if (newTransaction.TransactionType != '\0')
                oldTransaction.TransactionType = newTransaction.TransactionType;
            if (newTransaction.AccountNumber != 0)
                oldTransaction.AccountNumber = newTransaction.AccountNumber;
            if (newTransaction.DestinationAccountNumber != 0)
                oldTransaction.DestinationAccountNumber = newTransaction.DestinationAccountNumber;
            if (newTransaction.Amount != 0)
                oldTransaction.Amount = newTransaction.Amount;
            if (newTransaction.Comment != null)
                oldTransaction.Comment = newTransaction.Comment;
            if (newTransaction.TransactionTimeUtc != DateTime.MinValue)
                oldTransaction.TransactionTimeUtc = newTransaction.TransactionTimeUtc;
        }

        public static void Delete(McbaContext context, Transaction transaction)
        {
            var account = context.Accounts.Find(transaction.AccountNumber);
            account.Transactions.Remove(transaction);
        }
    }
}
