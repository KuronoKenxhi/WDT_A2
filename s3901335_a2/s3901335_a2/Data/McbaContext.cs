﻿using Microsoft.EntityFrameworkCore;
using s3901335_a2.Models;

namespace s3901335_a2.Data
{
    public class McbaContext : DbContext
    {
        public McbaContext(DbContextOptions<McbaContext> options) : base(options) 
        { }

        // List of Tables in DB 
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<Payee> Payee { get; set; }
        public DbSet<BillPay> BillPay { get; set; }

        // Fluent API
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Set check constraints (cannot be expressed with data annotations).
            builder.Entity<Login>().ToTable(b =>
            {
                b.HasCheckConstraint("CH_Login_LoginID", "len(LoginID) = 8");
                b.HasCheckConstraint("CH_Login_PasswordHash", "len(PasswordHash) = 94");
            });
            builder.Entity<Account>().ToTable(b => b.HasCheckConstraint("CH_Account_Balance", "Balance >= 0"));
            builder.Entity<Transaction>().ToTable(b => b.HasCheckConstraint("CH_Transaction_Amount", "Amount > 0"));

            // Configure ambiguous Account.Transactions navigation property relationship.
            //builder.Entity<Transaction>().
                //HasOne(x => x.Account).WithMany(x => x.Transactions).HasForeignKey(x => x.AccountNumber);
        }

    }
}
