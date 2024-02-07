using Microsoft.AspNetCore.Mvc;
using s3901335_a2.Models;
using s3901335_a2.Models.DataManager;

namespace s3901335_a2_WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerManager _repoCustomer;
    public CustomersController(CustomerManager repo)
    {
        _repoCustomer = repo;
    }
    // LOCK: api/Customers/lock
    [HttpPut("lock/{id}")]
    public int Lock([FromRoute] int id)
    {
        return _repoCustomer.Lock(id);
    }

    // GET ALL: api/Customers
    [HttpGet]
    public IEnumerable<Customer> Get()
    {
        return _repoCustomer.GetAll();
    }

    // GET: api/Customers/id
    [HttpGet("{id}")]
    public Customer Get(int id)
    {
        return _repoCustomer.Get(id);
    }

    // ADD: api/Customers
    [HttpPost]
    public void Post(Customer customer)
    {
        _repoCustomer.Add(customer);
    }

    // UPDATE: api/Customers
    [HttpPut]
    public void Put(Customer customer)
    {
        _repoCustomer.Update(customer.CustomerID, customer);
    }

    // DELETE: api/Customers/id
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        _repoCustomer.Delete(id);
    }
}
