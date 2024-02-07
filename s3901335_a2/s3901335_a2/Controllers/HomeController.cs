using Microsoft.AspNetCore.Mvc;
using s3901335_a2.Data;
using s3901335_a2.Models;
using System.Diagnostics;

namespace s3901335_a2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly McbaContext _context;

        public HomeController(ILogger<HomeController> logger, McbaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index() => View();

        public IActionResult Privacy() => View();


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
