using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using s3901335_a2.Models;
using System;

namespace s3901335_a2.Data
{
    public class SeedDataFromJson
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<McbaContext>();

            // Stop if content already exists in database
            if(context.Customers.Any())
            {
                return; //Db has content
            }

            const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

            using var client = new HttpClient();
            var json = client.GetStringAsync(Url).Result;

            List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(json, new JsonSerializerSettings
            {
                // Set DateTime format string
                DateFormatString = "dd/MM/yyyy hh:mm:ss tt"
            });

            PushToDb(context, customers);

        }

        public static void PushToDb(McbaContext context, List<Customer> customers)
        {
            // Insert deserialised json data into DB
            foreach (var customer in customers)
            {
                // Add customer to Customer Table
                context.Customers.Add(new Customer
                {
                    CustomerID = customer.CustomerID,
                    Name = customer.Name,
                    Address = customer.Address,
                    City = customer.City,
                    State = customer.State,
                    PostCode = customer.PostCode,
                });

                // Add login to login table
                context.Login.Add(new Login
                {
                    LoginID = customer.Login.LoginID,
                    CustomerID = customer.CustomerID,
                    PasswordHash = customer.Login.PasswordHash
                });


            }
            // Push Customers and Login to DB
            context.SaveChanges();

            foreach (var customer in customers)
            {
                foreach (var account in customer.Accounts)
                {
                    // Initialise variable for holding sum
                    decimal sumBalance = 0;

                    foreach (var transaction in account.Transactions)
                    {
                        // Add transaction amount to sum
                        if (transaction.Amount > 0)
                        {
                            sumBalance += transaction.Amount;
                        }

                        // Add transactions to transactions table
                        context.Transactions.Add(new Transaction
                        {
                            TransactionType = TransactionType.Deposit,
                            AccountNumber = account.AccountNumber,
                            Amount = transaction.Amount,
                            Comment = transaction.Comment,
                            TransactionTimeUtc = transaction.TransactionTimeUtc
                        });
                    }

                    // Update account Balance
                    account.Balance = sumBalance;

                    // Add accounts to accounts table
                    context.Accounts.Add(new Account
                    {
                        AccountNumber = account.AccountNumber,
                        AccountType = account.AccountType,
                        CustomerID = account.CustomerID,
                        Balance = sumBalance, 
                    });
                }
            }


            // SEED DATA FOR PAYEES
            context.Payee.Add(new Payee
            {
                Name = "Optus",
                Address = "12 Optus Street",
                City = "Melbourne",
                State = "VIC",
                PostCode = "3000",
                Phone = "(04) 1234 1234",
            });
            context.Payee.Add(new Payee
            {
                Name = "Telstra",
                Address = "666 Telstra Street",
                City = "Melbourne",
                State = "VIC",
                PostCode = "3000",
                Phone = "(04) 2345 2345",
            });
            context.Payee.Add(new Payee
            {
                Name = "Vodafone",
                Address = "3 Vodafone Street",
                City = "Melbourne",
                State = "VIC",
                PostCode = "3000",
                Phone = "(04) 3456 3456",
            });



            // Save Changes to Database to ensure that Transaction and Account records are inserted
            context.SaveChanges();

        }
    }
}
