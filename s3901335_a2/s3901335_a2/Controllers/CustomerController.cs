using Microsoft.AspNetCore.Mvc;
using s3901335_a2.Models;
using s3901335_a2.Data;
using s3901335_a2.Filters;
using Microsoft.EntityFrameworkCore;
using s3901335_a2.Utility;
using X.PagedList;
using SimpleHashing.Net;

namespace s3901335_a2.Controllers
{
    [AuthoriseCustomer] // Custom Authorisation Filter
    public class CustomerController : Controller
    {
        private readonly McbaContext _context;
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public CustomerController(McbaContext context)
        {
            _context = context;
        }

        // CUSTOMER HOME
        public async Task<IActionResult> Index()
        {
            // SHOWS ACCOUNTS BALANCE
            var customer = await _context.Customers.Include(x => x.Accounts).FirstOrDefaultAsync(x => x.CustomerID == CustomerID);
            return View(customer);
        }

        // DEPOSIT
        public async Task<IActionResult> Deposit(int id) => View(await _context.Accounts.FindAsync(id));
        [HttpPost]
        public async Task<IActionResult> Deposit(int id, decimal amount, string comment)
        {
            // DEPOSIT SYSTEM

            var account = await _context.Accounts.FindAsync(id);
            account.Transactions = _context.Transactions.Where(t => t.AccountNumber == account.AccountNumber).ToList();

            // VALIDATION
            if (amount <= 0)
            {
                ModelState.AddModelError(nameof(amount), "Deposit amount cannot be less than or equal to 0");
            }
            else if(amount.hasMoreThanTwoDecimalPlaces() == true)
            {
                ModelState.AddModelError(nameof(amount), "Deposit amount cannot have more than 2 decimal places"); 
            }
            if(comment.Length > 30)
            {
                ModelState.AddModelError(nameof(comment), "Comment has a 30 character limit");
            }

            // Check the model state for errors
            if(ModelState.IsValid == false)
            {
                ViewBag.Amount = amount;
                ViewBag.Comment = comment;
                return View(account);
            }

            // Proceed with the Deposit
            account.Balance = account.Balance + amount;
            account.Transactions.Add(new Transaction
            {
                TransactionType = TransactionType.Deposit,
                Amount = amount,
                Comment = comment,
                TransactionTimeUtc = DateTime.UtcNow
            });

            // Push Changes to DB
            await _context.SaveChangesAsync();

            // Redirect back to Index
            return RedirectToAction(nameof(Index));
        }

        // WITHDRAW
        public async Task<IActionResult> Withdraw(int id) => View(await _context.Accounts.FindAsync(id));
        [HttpPost]
        public async Task<IActionResult> Withdraw(int id, decimal amount, string comment)
        {
            // WITHDRAW SYSTEM

            var account = await _context.Accounts.FindAsync(id);
            account.Transactions = _context.Transactions.Where(t => t.AccountNumber == account.AccountNumber).ToList();

            // VALIDATION
            if (amount > account.Balance)
            {
                ModelState.AddModelError(nameof(amount), "Withdraw amount cannot exceed your current account balance");
            }
            else if (amount.hasMoreThanTwoDecimalPlaces() == true)
            {
                ModelState.AddModelError(nameof(amount), "Withdraw amount cannot have more than 2 decimal places");
            }
            if (comment.Length > 30)
            {
                ModelState.AddModelError(nameof(comment), "Comment has a 30 character limit");
            }

            // Check the model state for errors
            if (ModelState.IsValid == false)
            {
                ViewBag.Amount = amount;
                ViewBag.Comment = comment;
                return View(account);
            }

            // Proceed with the Withdrawal
            account.Balance = account.Balance - amount;
            account.Transactions.Add(new Transaction
            {
                TransactionType = TransactionType.Withdraw,
                Amount = amount,
                Comment = comment,
                TransactionTimeUtc = DateTime.UtcNow
            });

            // Push Changes to DB
            await _context.SaveChangesAsync();

            // Redirect back to Index
            return RedirectToAction(nameof(Index));
        }

        // TRANSFER
        public async Task<IActionResult> Transfer(int id) => View(await _context.Accounts.FindAsync(id));
        [HttpPost]
        public async Task<IActionResult> Transfer(int id, decimal amount, string comment,string target)
        {
            // Get Account and Transactions for Source Account
            var account = await _context.Accounts.FindAsync(id);
            account.Transactions = _context.Transactions.Where(t => t.AccountNumber == account.AccountNumber).ToList();

            // Get Target Account details
            var targetAccountNumber = int.Parse(target); // Convert string to int
            var targetAccount = await _context.Accounts.FindAsync(targetAccountNumber);
            

            // VALIDATION
                // Amount
            if (amount > account.Balance)
            {
                ModelState.AddModelError(nameof(amount), "Withdraw amount cannot exceed your current account balance");
            }
            else if (amount.hasMoreThanTwoDecimalPlaces() == true)
            {
                ModelState.AddModelError(nameof(amount), "Withdraw amount cannot have more than 2 decimal places");
            }
                // TargetAccount
            if(target == null)
            {
                ModelState.AddModelError(nameof(target), "The Target field is required");
            }
            else if(target.Length > 4)
            {
                ModelState.AddModelError(nameof(target), "The Target Account Number cannot be larger than 4 digits");
            }
            else if(targetAccountNumber == account.AccountNumber)
            {
                ModelState.AddModelError(nameof(target), "You cannot transfer to the same account");
            }
                // Comment
            if (comment.Length > 30)
            {
                ModelState.AddModelError(nameof(comment), "Comment has a 30 character limit");
            }

            // Check the model state for errors
            if (ModelState.IsValid == false)
            {
                ViewBag.Amount = amount;
                ViewBag.Comment = comment;
                ViewBag.Target = target;
                return View(account);
            }

            // Import the transactions for the account onto transactions property ( assumes valid account number )
            targetAccount.Transactions = _context.Transactions.Where(t => t.AccountNumber == targetAccount.AccountNumber).ToList();

            // Proceed with the Withdrawal

            // Source Transfer
            account.Balance = account.Balance - amount;
            account.Transactions.Add(new Transaction
            {
                TransactionType = TransactionType.Transfer,
                Amount = amount,
                DestinationAccountNumber = targetAccount.AccountNumber,
                Comment = comment,
                TransactionTimeUtc = DateTime.UtcNow
            });

            // Target Transfer
            targetAccount.Balance = targetAccount.Balance + amount;
            targetAccount.Transactions.Add(new Transaction
            {
                TransactionType = TransactionType.Transfer,
                Amount = amount,
                Comment = comment,
                TransactionTimeUtc = DateTime.UtcNow
            });

            // Push Changes to DB
            await _context.SaveChangesAsync();

            // Redirect back to Accounts Overview
            return RedirectToAction("Index", "Customer");
        }

        // MY STATEMENTS
        public async Task<IActionResult> MyStatements(int id) 
        {
            // Retrieve account
            var account = await _context.Accounts.FindAsync(id);
            

            // Check for null
            if (account == null)
            {
                return NotFound();
            }

            // Store account in session
            HttpContext.Session.SetInt32("SelectedAccount", id);
            return RedirectToAction(nameof(MyStatementsPage));
        } 
        public async Task<IActionResult> MyStatementsPage(int page = 1)
        {
            // Retrieve Account from session
            var account = await _context.Accounts.FindAsync(HttpContext.Session.GetInt32("SelectedAccount"));
            account.Transactions = await _context.Transactions.Where(x => x.AccountNumber == account.AccountNumber).ToListAsync();

            // Check for null/not set
            if (account == null)
            {
                // Redirect back to Index
                return RedirectToAction(nameof(Index));
            }

            // Add account to view bag
            ViewBag.Account = account;
            
            // Page the transactions
            const int pageSize = 4; // Number of transactions per page
            IPagedList<Transaction> pagedList = await _context.Transactions.Where(x => x.AccountNumber == account.AccountNumber).OrderByDescending(X => X.TransactionTimeUtc).ToPagedListAsync(page, pageSize);

            return View(pagedList);
        }

        // MY PROFILE
        public async Task<IActionResult> MyProfile()
        {
            // Pull Customer ID from session
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;
            var customer = await _context.Customers.FindAsync(customerID);

            // Return My Profile View and pass in the currently logged in customer
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile()
        {
            // Pull Customer ID from session
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;
            var customer = await _context.Customers.FindAsync(customerID);

            // Return My Profile edit page
            return View(customer);
        }

        public async Task<IActionResult> UpdateProfile(Customer updatedCustomer)
        {
            // Pull Customer ID from session
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;
            var customer = await _context.Customers.FindAsync(customerID);

            // VALIDATION
            if(string.IsNullOrEmpty(updatedCustomer.Name))
            {
                ModelState.AddModelError(nameof(updatedCustomer.Name), "Name cannot be empty");
            }
            else if(updatedCustomer.Name.Any(char.IsDigit))
            {
                ModelState.AddModelError(nameof(updatedCustomer.Name), "Name cannot contain any digits");
            }

            if (ModelState.IsValid == false)
            {
                updatedCustomer.CustomerID = customerID;
                return View("EditProfile", updatedCustomer);
            }

            // Update customer properties with updated values
            customer.Name = updatedCustomer.Name;
            customer.TFN = updatedCustomer.TFN;
            customer.Address = updatedCustomer.Address;
            customer.City = updatedCustomer.City;
            customer.State = updatedCustomer.State;
            customer.PostCode = updatedCustomer.PostCode;
            customer.Mobile = updatedCustomer.Mobile;

            // Push Changes to DB
            await _context.SaveChangesAsync();

            // Return to My Profile View and pass in the currently logged in customer
            return RedirectToAction("MyProfile", "Customer");
        }

        // CHANGE PASSWORD
        public async Task<IActionResult> ChangePassword(int id) => View(await _context.Login.FirstOrDefaultAsync(x => x.CustomerID == id));
        [HttpPost]
        public async Task<IActionResult> ChangePassword(int id, string currentPassword, string newPassword, string newPassword2)
        {
            ISimpleHash simpleHash = new SimpleHash();
            // Retrieve Login data from DB
            var login = await _context.Login.FirstOrDefaultAsync(x => x.CustomerID == id);

            // VALIDATION

            // Check current password
            if (simpleHash.Verify(currentPassword, login.PasswordHash) == false)
            {
                ModelState.AddModelError("CurrentPassword", "Invalid Password");
            }

            // Check to ensure re-entered password matches the first input
            if(!newPassword.Equals(newPassword2))
            {
                ModelState.AddModelError("NewPassword2", "The new passwords must match");
            }

            if(ModelState.IsValid == false)
            {
                // Return change password page without prefilled fields
                return View(login);
            }

            // Proceed to Update Login in DB

            // TODO: UPDATE DB WITH HASHED PASSWORD
            var hashedPassword = simpleHash.Compute(newPassword);
            login.PasswordHash = hashedPassword;

            await _context.SaveChangesAsync();
            
            // Redirect back to My Profile
            return RedirectToAction("MyProfile", "Customer");
        }

    }
}
