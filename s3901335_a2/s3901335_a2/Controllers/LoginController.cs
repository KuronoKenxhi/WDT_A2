using Microsoft.AspNetCore.Mvc;
using s3901335_a2.Models;
using s3901335_a2.Data;
using SimpleHashing.Net;

namespace s3901335_a2.Controllers
{
    public class LoginController : Controller
    {
        private static readonly ISimpleHash s_simpleHash = new SimpleHash();
        private readonly McbaContext _context;

        public LoginController(McbaContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string loginID, string password)
        {
            // Search Login table for record with matching loginID
            var login = await _context.Login.FindAsync(loginID);

            // Get customer to check if locked
            var customer = _context.Customers.Find(login.CustomerID);
            // Check Lock State of customer
            if(customer.LockState == LockState.Locked)
            {
                // Add an error to model state and redirect back to Login page
                ModelState.AddModelError("LoginFailed", "Login failed, account is locked.");
                return View(new Login { LoginID = loginID });
            }

            // Check if record was found, and whether or not the password is correct
            if (login == null || string.IsNullOrEmpty(password) || !s_simpleHash.Verify(password, login.PasswordHash))
            {
                // Add an error to model state and redirect back to Login page
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                return View(new Login { LoginID = loginID });
            }

            // Login customer.
            HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);

            // Pull customer name from db and store in session for later use
            var customerName = customer.Name;
            HttpContext.Session.SetString(nameof(customerName), customerName);

            // Redirect to Customer Index page
            return RedirectToAction("Index", "Customer");
        }

        [Route("LogOutNow")]
        public IActionResult Logout()
        {
            // Clear session variables
            HttpContext.Session.Clear();

            // Redirect back to home route
            return RedirectToAction("Index", "Home");
        }
    }
}
