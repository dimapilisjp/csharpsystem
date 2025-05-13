using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.RegistrationModel;
using System.Text.Json;

namespace votingsystem.Pages.Shared
{
    public class OTPPageModel : PageModel
    {
        [BindProperty]
        public string OtpCode { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public User UserInfo { get; set; }

        public void OnGet()
        {
            
            Email = TempData["Email"] as string;

            var userJson = TempData["User"] as string;
            if (!string.IsNullOrEmpty(userJson))
            {
                UserInfo = JsonSerializer.Deserialize<User>(userJson);
                TempData["User"] = userJson; 
            }

            TempData["Email"] = Email; 
        }


        public IActionResult OnPostVerify()
        {
            // to make email available for OTP and user data
            Email = TempData["Email"] as string;
            var userJson = TempData["User"] as string;

            if (votingsystem.Database_Helper.DbHelper.ValidateOTP(Email, OtpCode)) //DbHelper #51
            {
                // deserialize user info from tempdata
                if (!string.IsNullOrEmpty(userJson))
                {
                    UserInfo = JsonSerializer.Deserialize<User>(userJson);

                    // will only register the user after OTP is verified
                    if (UserInfo != null)
                    {
                        Database_Helper.DbHelper.RegisterUser(UserInfo); //DbHelper #1

                        TempData["Message"] = "Account created successfully. Please wait for admin approval.";
                        return RedirectToPage("/Registration");
                    }
                }

                ModelState.AddModelError(string.Empty, "User information is missing. Please register again.");
                return RedirectToPage("/Registration");
            }
            else
            {
                // restore tempdata to be available on next request
                TempData["Email"] = Email;
                TempData["User"] = userJson;

                ModelState.AddModelError(string.Empty, "Invalid or expired OTP.");
                return Page();
            }
        }
    }
}
