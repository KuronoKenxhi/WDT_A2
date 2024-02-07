using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using s3901335_a2.Data;
using s3901335_a2.Filters;
using s3901335_a2.Models;
using s3901335_a2.Utility;
using X.PagedList;

namespace s3901335_a2.Controllers;

[AuthoriseCustomer]
public class BillPayController : Controller
{
    private readonly McbaContext _context;
    private int _customerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public BillPayController(McbaContext context)
    {
        _context = context;
    }

    // BILLPAY
    public async Task<IActionResult> BillPay(int id)
    {
        // Retrieve Customer
        var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32("CustomerID"));

        // Retrieve list of scheduled billpay transfers from DB for all accounts
        var billPayTransfers = await _context.BillPay.Where(x => x.Account.CustomerID == customer.CustomerID).ToListAsync();

        // Check for null
        if (billPayTransfers == null)
        {
            return NotFound();
        }

        // Redirect to new action method
        return RedirectToAction(nameof(BillPayTransfers));
    }
    // BILL OVERVIEW
    public async Task<IActionResult> BillPayTransfers(int id, int page = 1)
    {
        // Retrieve Account from session
        var customer = await _context.Customers.FindAsync(HttpContext.Session.GetInt32("CustomerID"));

        // Check for null/not set
        if (customer == null)
        {
            // Redirect back to Index
            return RedirectToAction(nameof(Index));
        }

        // Add account to view bag
        ViewBag.CustomerID = customer.CustomerID;

        // Page the transactions
        const int pageSize = 4; // Number of scheduled payments per page
        IPagedList<BillPay> pagedList = await _context.BillPay.Where(x => x.Account.CustomerID == customer.CustomerID).OrderByDescending(X => X.ScheduleTimeUtc).ToPagedListAsync(page, pageSize);

        return View(pagedList);
    }

    // ADD BILL
    public async Task<IActionResult> AddBill()
    {
        // Retrieve all required information to prefill fields
        var listOfAccNumbers = await _context.Customers.Where(x => x.CustomerID == _customerID).SelectMany(c => c.Accounts.Select(a => a.AccountNumber)).ToListAsync(); 
        var payeeNames = await _context.Payee.Select(x => x.Name).ToListAsync();
        var viewModel = new AddBillViewModel
        {
            AccountNumber = 0,
            PayeeID = 0,
            Amount = 0.00m,
            ScheduleTimeUtc = DateTime.Now,
            AccountNumbers = listOfAccNumbers,
            PayeeNames = payeeNames,
        };
        return View(viewModel);
    }
    [HttpPost]
    public async Task<IActionResult> AddBill(AddBillViewModel viewModel)
    {
        // Get account selected
        var account = await _context.Accounts.FindAsync(viewModel.AccountNumber);

        // Get list of all payees from DB
        var payeesNamesList = await _context.Payee.Select(x => x.Name).ToListAsync();

        // Set the utc time
        viewModel.ScheduleTimeUtc = viewModel.ScheduleTimeLocal.ToUniversalTime();
        
        // VALIDATION
        if (viewModel.AccountNumber == 0)
        {
            ModelState.AddModelError("AccountNumber", "Must select an account number");
        }
        if(viewModel.PayeeName == "0")
        {
            ModelState.AddModelError("PayeeID", "Must select a Payee");
        }
        else
        {
            // Find and get the payee Id from the name
            viewModel.PayeeID = _context.Payee.Where(x => x.Name == viewModel.PayeeName).First().PayeeID;
        }
        if (viewModel.Amount <= 0)
        {
            ModelState.AddModelError("Amount", "Amount cannot be 0");
        }
        else if(viewModel.Amount.hasMoreThanTwoDecimalPlaces() == true)
        {
            ModelState.AddModelError("Amount", "Amount cannot be more than two decimal places");
        }
        if(viewModel.ScheduleTimeLocal < DateTime.Now)
        {
            ModelState.AddModelError("ScheduleTimeLocal", "Schedule date must be in th future");
        }
        if(viewModel.Period == 0)
        {
            ModelState.AddModelError("Period", "Must select a Bill Period");
        }


        // Check model state for errors
        if (ModelState.IsValid == false)
        {
            // Return to Add Bill Page with prefilled fields
            viewModel.PayeeNames = await _context.Payee.Select(x => x.Name).ToListAsync();
            viewModel.AccountNumbers = await _context.Customers.Where(x => x.CustomerID == _customerID).SelectMany(c => c.Accounts.Select(a => a.AccountNumber)).ToListAsync();
            return View(viewModel);
        }

        // Append Bill to BillPay in DB
        _context.BillPay.Add(new BillPay
        {
            AccountNumber = viewModel.AccountNumber,
            PayeeID = viewModel.PayeeID,
            Amount = viewModel.Amount,
            ScheduleTimeUtc = viewModel.ScheduleTimeUtc,
            Period = viewModel.Period
        });

        // Push Bill to DB
        await _context.SaveChangesAsync();

        return RedirectToAction("BillPay", "BillPay");
    }

    // CANCEL BILL
    public async Task<IActionResult> ConfirmCancelBill(int id)
    {
        var bill = await _context.BillPay.FindAsync(id);

        if (bill == null)
        {
            return NotFound();
        }

        return View(bill);
    }
    [HttpPost]
    public async Task<IActionResult> CancelBill(int id)
    {
        // Get bill from id
        var bill = await _context.BillPay.FindAsync(id);

        if (bill != null)
        {
            // Remove from db
            _context.BillPay.RemoveRange(bill);

            // Push changes to DB
            await _context.SaveChangesAsync();

            // Redirect back to billpay overview
            return RedirectToAction("BillPay");
        }
        else
        {
            // Handle the case where the bill with the specified ID was not found
            return NotFound();
        }
    }
}
