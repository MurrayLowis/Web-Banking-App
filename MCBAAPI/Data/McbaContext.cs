﻿using Microsoft.EntityFrameworkCore;
using MCBAAPI.Models;

namespace MCBAAPI.Data;

public class McbaContext : DbContext
{
    public McbaContext(DbContextOptions<McbaContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Payee> Payees { get; set; }
    public DbSet<BillPay> BillPays { get; set; }

    // Fluent-API.
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Set check constraints (cannot be expressed with data annotations).
        builder.Entity<Login>().HasCheckConstraint("CH_Login_LoginID", "len(LoginID) = 8").
            HasCheckConstraint("CH_Login_PasswordHash", "len(PasswordHash) = 64");
        builder.Entity<Transaction>().HasCheckConstraint("CH_Transaction_Amount", "Amount > 0");
        // source account and destination account cannot be the same
        builder.Entity<Transaction>().HasCheckConstraint("CH_Transaction_DestinationAccountNumber", "DestinationAccountNumber != AccountNumber");

        // Configure ambiguous Account.Transactions navigation property relationship.
        builder.Entity<Transaction>().HasOne(x => x.Account).WithMany(x => x.Transactions).HasForeignKey(x => x.AccountNumber);

        // set PayeeID from Payee as FK for BillPays
        builder.Entity<BillPay>().HasOne(x => x.Payee).WithMany(x => x.BillPays).HasForeignKey(x => x.PayeeID);
    }
}
