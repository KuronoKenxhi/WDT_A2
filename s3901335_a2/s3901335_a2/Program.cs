// Web Development Technologies Assignment 2
// Mitchell Hughes s3901335
// Email: s3901335@student.rmit.edu.au
// Creation Date: 16/01/2024

using Microsoft.EntityFrameworkCore;
using s3901335_a2.Data;
using s3901335_a2.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<McbaContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("McbaContext")));

// Add the billpay background service
builder.Services.AddHostedService<BillPayBackgroundService>();

// Store session into web server memory
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
});

// Add Controllers with views
builder.Services.AddControllersWithViews();

// Build Web App
var app = builder.Build();

// Import Data from Json into DB
using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedDataFromJson.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while importing json data into the DB");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
