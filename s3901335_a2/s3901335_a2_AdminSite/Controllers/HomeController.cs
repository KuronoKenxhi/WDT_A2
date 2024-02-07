using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using s3901335_a2_AdminSite.Filters;
using s3901335_a2_AdminSite.Models;

namespace s3901335_a2_AdminSite.Controllers;

[AuthoriseAdmin]
public class HomeController : Controller
{
    private readonly HttpClient _client;
    public HomeController(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("api");
    }

    // LIST OF CUSTOMERS
    public async Task<IActionResult> Index()
    {
        // Send a request to the API to get all customers
        var response = await _client.GetAsync("api/Customers");

        // Ensure the response is successful
        response.EnsureSuccessStatusCode();

        // Store result as a string and deserialise into an object
        string result = await response.Content.ReadAsStringAsync();
        var customers = JsonConvert.DeserializeObject<List<Customer>>(result);

        // Pass the model to the view
        return View(customers);
    }
    public async Task<IActionResult> EditCustomer(int id)
    {
        // Get Customer data by ID and deserialise into customer object
        var response = await _client.GetAsync($"api/Customers/{id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        var customer = JsonConvert.DeserializeObject<Customer>(result);

        // Return view with customer object
        return View(customer);
    } 
    [HttpPost]
    public async Task<IActionResult> EditCustomer(int id, Customer updatedCustomer)
    {
        // Check Model State
        if(ModelState.IsValid == false)
        {
            return View(updatedCustomer);
        }

        // Get customer by ID and deserialise
        var response = await _client.GetAsync($"api/Customers/{id}");
        response.EnsureSuccessStatusCode();
        string result = await response.Content.ReadAsStringAsync();
        var customer = JsonConvert.DeserializeObject<Customer>(result);

        // Update Customer
        customer.Name = updatedCustomer.Name;
        customer.TFN = updatedCustomer.TFN;
        customer.Address = updatedCustomer.Address;
        customer.City = updatedCustomer.City;
        customer.State = updatedCustomer.State;
        customer.PostCode = updatedCustomer.PostCode;
        customer.Mobile = updatedCustomer.Mobile;

        // Serialise content and pass to put request
        var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, MediaTypeNames.Application.Json);
        var response2 = await _client.PutAsync("api/Customers", content);

        response2.EnsureSuccessStatusCode();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> LockCustomer(int id)
    {
        // Get customer by ID and deserialise
        var response = await _client.GetAsync($"api/Customers/{id}");
        response.EnsureSuccessStatusCode();
        string result = await response.Content.ReadAsStringAsync();
        var customer = JsonConvert.DeserializeObject<Customer>(result);

        // Return Confirm screen with customer details
        return View(customer);
    }
    [HttpPost]
    public async Task<IActionResult> ActuallyLockCustomer(int id)
    {
        // Get customer by ID and deserialise
        var response = await _client.PutAsync($"api/Customers/lock/{id}", null);
        response.EnsureSuccessStatusCode();

        // Return Confirm screen with customer details
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
