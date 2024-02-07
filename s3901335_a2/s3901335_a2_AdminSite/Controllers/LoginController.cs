using Microsoft.AspNetCore.Mvc;
using s3901335_a2_AdminSite.Models;
using SimpleHashing.Net;

namespace s3901335_a2_AdminSite.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _client;
        public LoginController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("api");
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string loginID, string password)
        {
            // Check login details against hard coded admin login
            if(loginID != "admin" && password != "admin")
            {
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                return View();
            }

            // Login Admin.
            HttpContext.Session.SetInt32("adminStatus", 1);

            // Redirect to Admin Index page
            return RedirectToAction("Index", "Home");
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
