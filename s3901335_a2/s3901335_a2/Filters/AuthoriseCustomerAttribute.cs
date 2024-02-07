using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using s3901335_a2.Models;

namespace s3901335_a2.Filters
{
    public class AuthoriseCustomerAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get customer id from session variable
            var customerID = context.HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            // Check if it has a value
            if(!customerID.HasValue)
            {
                // If no value is present, redirect back to home
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
