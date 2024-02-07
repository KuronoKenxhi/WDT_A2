using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using s3901335_a2.Data;
using s3901335_a2.Models;

namespace s3901335_a2.BackgroundServices;
public class BillPayBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BillPayBackgroundService> _logger;

    public BillPayBackgroundService(IServiceProvider services, ILogger<BillPayBackgroundService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("BillPay background service is running.");

        while(!cancellationToken.IsCancellationRequested)
        {
            // PROCESS BILLS ANY BILLS THAT NEED TO BE PROCESSED
            await ProcessBills(cancellationToken);

            // Delay by 10 seconds unless cancellation is requested
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        }
    }

    private async Task ProcessBills(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing Bills");

        // Create scope and retrieve the DB Context
        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<McbaContext>();

        // Get all bills that are past the schedule date
        var bills = await context.BillPay.Where(x => x.ScheduleTimeUtc <= DateTime.UtcNow).ToListAsync();

        foreach(var bill in bills)
        {
            // Process the bills and either remove them from the db or refresh with new datetime

            // Get customer account
            var account = context.Accounts.Where(x => x.AccountNumber == bill.AccountNumber).First();
            // Get Payee details for displaying name later
            var payee = context.Payee.Where(x => x.PayeeID == bill.PayeeID).First();

            
            // TODO: Validate amount against remaining balance
            if(bill.Amount > account.Balance)
            {
                _logger.LogWarning("Account could not afford the bill");
            }

            // Update balance
            account.Balance = account.Balance - bill.Amount;

            // Normally would need to send the money to the payee and update their balance aswell

            // Add a new transaction of type 'b' to the Transactions Table
            await context.Transactions.AddAsync(new Transaction
            {
                TransactionType = TransactionType.BillPay,
                AccountNumber = bill.AccountNumber,
                Amount = bill.Amount,
                Comment = $"Bill To {payee.Name}",
                TransactionTimeUtc = bill.ScheduleTimeUtc
            });



            // If Bill is recurring, reinstate the bill with a new scheduled date
            if(bill.Period == Period.Monthly)
            {
                // Create a new Bill thats identical to the current one, but date is pushed back one month
                context.BillPay.Add(new BillPay
                {
                    AccountNumber = account.AccountNumber,
                    PayeeID = payee.PayeeID,
                    Amount = bill.Amount,
                    ScheduleTimeUtc = bill.ScheduleTimeUtc.AddMonths(1),
                    Period = Period.Monthly,
                });

                // And Remove the old Bill from the bill pay table
                context.BillPay.RemoveRange(bill);
            }
            else
            {
                // Remove the one off payments from the bills list
                context.BillPay.RemoveRange(bill);
            }

            // Push changes to DB
            await context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("All Pending Bills Processed");
    }
}
