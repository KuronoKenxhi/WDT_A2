using s3901335_a2.Data;
using s3901335_a2.Models;
using s3901335_a2.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Sdk;

namespace s3901335_a2.Tests;

public class CustomerControllerTests : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly McbaContext _context;

    public CustomerControllerTests()
    {
        // Create services collection
        var services = new ServiceCollection();

        // Register Db Context etc 
        services.AddDbContext<McbaContext>(options => options.UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared"));

        // Build Service Provider
        _serviceProvider = services.BuildServiceProvider();

        // Create context from service provider
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<McbaContext>();

        // Bypass Migrations for short term db solely for testing
        context.Database.EnsureCreated();

        // Seed Data from JSON
        SeedDataFromJson.Initialize(_serviceProvider);

        // Store context in private variable
        _context = context;
    }

    [Fact]
    public async Task Index_Returns_View_ListOfAccounts()
    {
        // Arrange
        var controller = new CustomerController(_context);

        // Act
        IActionResult result = await controller.Index();

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Customer>(viewResult.ViewData.Model);

        // This doesnt do anything
        Assert.Equal(model, model);
    }




    // Dispose Method
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
