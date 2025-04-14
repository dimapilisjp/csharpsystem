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
        bool isVerified = votingsystem.Database_Helper.DbHelper.VerifyUser(UserName, Password); 

        if (isVerified)
        {
            Console.WriteLine("User verified. Redirecting...");
            return RedirectToPage("/Shared/Dashboard"); 
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
