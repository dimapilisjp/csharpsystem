using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace votingsystem.Pages.Shared
{
    public class ResetPasswordModel : PageModel
    {
        public string Token { get; set; }
        public string Message { get; set; }

        public void OnGet(string token)
        {
            Token = token;

            // validate token
            bool isValid = Database_Helper.DbHelper.ValidateResetToken(token); //DbHelper #8
            if (!isValid)
            {
                Message = "Invalid or expired reset token.";
            }
        }
        public IActionResult OnPostResetPassword(string token, string password, string confirmPassword)
        {
            // validate password 
            if (string.IsNullOrWhiteSpace(password) || password != confirmPassword)
            {
                Message = "Passwords do not match.";
                return Page();
            }

            // reset password
            bool success = Database_Helper.DbHelper.ResetPassword(token, password); //DbHelper #9
            if (success)
            {
                Message = "Password successfully reset!";
                return RedirectToPage("/Index", null, null, "https");

            }
            else
            {
                Message = "Failed to reset password. Invalid or expired token.";
                return Page();
            }
        }
    }
}
