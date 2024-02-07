using s3901335_a2.Models.Repository;
using s3901335_a2.Data;


namespace s3901335_a2.Models.DataManager;

public class CustomerManager : IDataRepository<Customer, int>
{
    private readonly McbaContext _context;

    public CustomerManager(McbaContext context)
    {
        _context = context;
    }

    // Checks for Existing Customer
    public Boolean Exists(int id)
    {
        var customer = _context.Customers.Find(id);
        if(customer == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    // Locks a customer and unlocks if already locked
    public int Lock(int id)
    {
        var customer = _context.Customers.Find(id);

        // Check if customer is already locked
        if(customer.LockState == LockState.Locked)
        {
            // Unlock the customer
            customer.LockState = LockState.Unlocked;
        }
        else
        {
            // Otherwise lock the customer
            customer.LockState = LockState.Locked;
        }

        // Push to DB
        _context.SaveChanges();

        // Return the id of the customer that was locked/unlocked
        return id;
    }

    // Gets Customer by CustomerID
    public Customer Get(int id)
    {
        return _context.Customers.Find(id);
    }

    // Gets all customers as a list
    public IEnumerable<Customer> GetAll()
    {
        return _context.Customers.ToList();
    }

    // Adds a customer
    public int Add(Customer customer)
    {
        // Add new customer
        _context.Customers.Add(customer);
        _context.SaveChanges();

        return customer.CustomerID;
    }

    // Deletes a customer by CustomerID
    public int Delete(int id)
    {
        // Remove customer by id from DB
        _context.Customers.RemoveRange(_context.Customers.Find(id));
        _context.SaveChanges();

        return id;
    }

    // Updates a customer
    public int Update(int id, Customer newCustomer)
    {
        var existingCustomer = _context.Customers.Find(id);
        
        // Update the customer with new values
        existingCustomer.Name = newCustomer.Name;
        existingCustomer.TFN = newCustomer.TFN;
        existingCustomer.Address = newCustomer.Address;
        existingCustomer.City = newCustomer.City;
        existingCustomer.PostCode = newCustomer.PostCode;
        existingCustomer.Mobile = newCustomer.Mobile;

        // Push changes to DB
        _context.SaveChanges();

        return id;
    }
}
