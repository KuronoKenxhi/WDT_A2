using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using s3901335_a2_AdminSite.Models;

namespace s3901335_a2_AdminSite.Filters
{
    public class AuthoriseAdminAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get customer id from session variable
            var adminID = context.HttpContext.Session.GetInt32("adminStatus");
            // Check if it has a value
            if(adminID != 1)
            {
                // If no value is present, redirect back to login page
                context.Result = new RedirectToActionResult("Login", "Login", null);
            }
        }
    }
}
