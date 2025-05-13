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

        // validate username and password
        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
        {
            Console.WriteLine("Username or Password is empty.");
            Message = "Username and Password are required.";
            return Page();
        }

        Console.WriteLine($"Username: {UserName}");
        Console.WriteLine($"Password: {Password}");

        // verify user's details and approval status
        Console.WriteLine("Calling VerifyUser...");
        bool isVerified = Database_Helper.DbHelper.VerifyUser(UserName, Password); //DbHelper #4

        if (isVerified)
        {
            Console.WriteLine("User verified. Checking approval status...");

            // check if the user is approved
            bool isApproved = Database_Helper.DbHelper.IsUserApproved(UserName); //DbHelper #5

            if (!isApproved)
            {
                Console.WriteLine($"Login blocked: User {UserName} is not approved.");
                Message = "Your account is awaiting approval. Please wait for admin approval.";
                return Page(); 
            }

            Console.WriteLine("User approved. Fetching user role...");

            // fetch role of the user
            string role = Database_Helper.DbHelper.GetUserRole(UserName); //DbHelper #10

            Console.WriteLine($"User role: {role}");

            //sSet claims for authentication
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, UserName), // sets User.Identity.Name
            new Claim(ClaimTypes.Role, role)     // claims role of user
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // issue the authentication cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            Console.WriteLine($"[Login] Authentication cookie issued for UserName={UserName}, Role={role}");

            // redirect based on user role
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
            
            if (!Database_Helper.DbHelper.IsUserApproved(UserName)) //DbHelper #5
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
