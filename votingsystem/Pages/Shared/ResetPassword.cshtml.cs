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

        // GET: Display Reset Password Form
        public void OnGet(string token)
        {
            Token = token;

            // Validate the token
            bool isValid = Database_Helper.DbHelper.ValidateResetToken(token);
            if (!isValid)
            {
                Message = "Invalid or expired reset token.";
            }
        }

        // POST: Send Reset Password Link
        //public IActionResult OnPostSendResetLink(string email)
        //{
        //    // Validate email input
        //    if (string.IsNullOrWhiteSpace(email))
        //    {
        //        Message = "Please enter a valid email address.";
        //        return Page();
        //    }

        //    // Check if email exists in the database
        //    bool userExists = Database_Helper.DbHelper.CheckIfEmailExist(email);
        //    if (!userExists)
        //    {
        //        Message = "Email not found. Please try again.";
        //        return Page();
        //    }

        //    // Generate a secure reset link
        //    string resetToken = Guid.NewGuid().ToString(); // Unique token
        //    string resetUrl = Url.Page("/Shared/ResetPassword", null, new { token = resetToken }, Request.Scheme);

        //    // Save reset token to the database
        //    Database_Helper.DbHelper.SaveResetToken(email, resetToken);

        //    // Send the reset email
        //    try
        //    {
        //        SendResetEmail(email, resetUrl);
        //        Message = "A password reset link has been sent to your email address.";
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error while sending email: {ex.Message}");
        //        Message = "Failed to send reset link. Please try again later.";
        //    }

        //    return Page();
        //}

        // POST: Reset Password
        public IActionResult OnPostResetPassword(string token, string password, string confirmPassword)
        {
            // Validate password match
            if (string.IsNullOrWhiteSpace(password) || password != confirmPassword)
            {
                Message = "Passwords do not match.";
                return Page();
            }

            // Reset the password
            bool success = Database_Helper.DbHelper.ResetPassword(token, password);
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

        //// Helper: Send Password Reset Email
        //private void SendResetEmail(string email, string resetUrl)
        //{
        //    var smtpClient = new SmtpClient("smtp.your-email-provider.com")
        //    {
        //        Port = 587,
        //        Credentials = new NetworkCredential("your-email@example.com", "your-email-password"),
        //        EnableSsl = true
        //    };

        //    var mailMessage = new MailMessage
        //    {
        //        From = new MailAddress("your-email@example.com"),
        //        Subject = "Password Reset Request",
        //        Body = $"Click the link to reset your password: <a href=\"{resetUrl}\">{resetUrl}</a>",
        //        IsBodyHtml = true,
        //    };
        //    mailMessage.To.Add(email);

        //    smtpClient.Send(mailMessage);
        //}
    }
}
