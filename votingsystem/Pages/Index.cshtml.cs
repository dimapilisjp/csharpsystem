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
    public string Message { get; set; }
    public IActionResult OnPostLogin()
    {
        Console.WriteLine("OnPostLogin method executed.");

        Console.WriteLine($"Username: {UserName}");
        Console.WriteLine($"Password: {Password}");

        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
        {
            Console.WriteLine("Username or Password is empty.");
            Message = "Username and Password are required.";
            return Page();
        }

        Console.WriteLine("Calling VerifyUser...");
        bool isVerified = Database_Helper.DbHelper.VerifyUser(UserName, Password);

        if (isVerified)
        {
            Console.WriteLine("User verified. Fetching user role...");

            // fetch user's role 
            string role = Database_Helper.DbHelper.GetUserRole(UserName);

            Console.WriteLine($"User role: {role}");

            // redirect based on  role
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
                return RedirectToPage("/Shared/DefaultPage");
            }
        }
        else
        {
            Console.WriteLine("User verification failed.");
            Message = "Invalid Username or Password.";
            return Page();
        }
    }




    public IActionResult OnPostRedirectToRegistration()
    {
        Console.WriteLine("Redirecting to Registration...");
        return RedirectToPage("/Registration"); 
    }



    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
