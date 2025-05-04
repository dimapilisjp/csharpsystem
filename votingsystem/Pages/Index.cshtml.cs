using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using votingsystem.Pages.Shared;

namespace votingsystem.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string UserName { get; set; } 

    [BindProperty]
    public string Password { get; set; }
    [BindProperty]
    public string Message { get; set; }
    public IActionResult OnPostRedirectToForgotPassword()
    {
        Console.WriteLine("Redirecting to Forgot Password...");
        return RedirectToPage("/Shared/ForgotPassword");
    }
    public IActionResult OnPostRedirectToRegistration()
    {
        Console.WriteLine("Redirecting to Registration...");
        return RedirectToPage("/Registration"); 
    }
    public async Task<IActionResult> OnPostAsync()
    {
        Console.WriteLine("OnPostLoginAsync method executed.");

        // Validate that Username and Password are provided
        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
        {
            Console.WriteLine("Username or Password is empty.");
            Message = "Username and Password are required.";
            return Page();
        }

        Console.WriteLine($"Username: {UserName}");
        Console.WriteLine($"Password: {Password}");

        // Verify user's credentials and approval status
        Console.WriteLine("Calling VerifyUser...");
        bool isVerified = Database_Helper.DbHelper.VerifyUser(UserName, Password);

        if (isVerified)
        {
            Console.WriteLine("User verified. Checking approval status...");

            // Check if the user is approved
            bool isApproved = Database_Helper.DbHelper.IsUserApproved(UserName); // New method in DbHelper

            if (!isApproved)
            {
                Console.WriteLine($"Login blocked: User {UserName} is not approved.");
                Message = "Your account is awaiting approval. Please wait for admin approval.";
                return Page(); // Block login
            }

            Console.WriteLine("User approved. Fetching user role...");

            // Fetch the user's role from the database
            string role = Database_Helper.DbHelper.GetUserRole(UserName);

            Console.WriteLine($"User role: {role}");

            // Set claims for authentication
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, UserName), // Sets User.Identity.Name
            new Claim(ClaimTypes.Role, role)     // Adds user's role as a claim
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Issue the authentication cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            Console.WriteLine($"[Login] Authentication cookie issued for UserName={UserName}, Role={role}");

            // Redirect based on user role
            if (role == "Admin")
            {
                Console.WriteLine("Redirecting to Dashboard...");
                return RedirectToPage("/Shared/Dashboard");
            }
            else if (role == "User")
            {
                Console.WriteLine("Redirecting to User Page...");
                return RedirectToPage("/Shared/UserPage");
            }
            else
            {
                Console.WriteLine("Unknown role. Redirecting to default page...");
                return RedirectToPage("/Index");
            }
        }
        else
        {
            // Determine the message for the login failure
            if (!Database_Helper.DbHelper.IsUserApproved(UserName)) // Assuming IsUserApproved is a new DbHelper method
            {
                Message = "Your account is awaiting approval. Please wait for admin approval.";
            }
            else
            {
                Message = "Invalid Username or Password. Please try again.";
            }

            Console.WriteLine("Login failed.");
            return Page(); 
        }
    }







    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        Console.WriteLine($"User.Identity.IsAuthenticated: {User.Identity.IsAuthenticated}");
        Console.WriteLine($"User.Identity.Name: {User.Identity.Name}");
    }
}
