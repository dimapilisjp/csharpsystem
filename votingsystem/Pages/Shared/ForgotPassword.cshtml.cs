using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace votingsystem.Pages.Shared
{
    public class ForgotPasswordModel : PageModel
    {
        public string Message { get; set; }

        public IActionResult OnPostForgotPassword(string email)
        {
            // validate email
            if (string.IsNullOrWhiteSpace(email)) return BadRequest("Invalid email address.");

            if (!Database_Helper.DbHelper.CheckIfEmailExist(email))
                return BadRequest("Email not found.");

            // generate token and save
            string token = Guid.NewGuid().ToString();
            Database_Helper.DbHelper.SaveResetToken(email, token);

            // send reset link
            string resetLink = Url.Page("/ResetPassword", null, new { token }, Request.Scheme);
            SendResetEmail(email, resetLink);

            Message = "Reset link sent to email.";
            return RedirectToPage("/Index");
        }

        public IActionResult OnPostSendResetLink(string Email)
        {
            // validate the entered email
            if (string.IsNullOrWhiteSpace(Email))
            {
                Message = "Please enter a valid email address.";
                return Page();
            }

            // check if the email exists in the database
            bool userExists = Database_Helper.DbHelper.CheckIfEmailExist(Email);
            if (!userExists)
            {
                Message = "Email not found. Please try again.";
                return Page();
            }

            // generates reset token and URL
            string resetToken = Guid.NewGuid().ToString(); 
            string resetUrl = Url.Page("/Shared/ResetPassword", null, new { token = resetToken }, "https"); 
            Console.WriteLine($"Generated Reset URL: {resetUrl}");
            SendResetEmail(Email, resetUrl);

            // save the reset token
            try
            {
                Database_Helper.DbHelper.SaveResetToken(Email, resetToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving reset token: {ex.Message}");
                Message = "An error occurred while processing your request. Please try again later.";
                return Page();
            }

            // send reset link via email
            try
            {
                SendResetEmail(Email, resetUrl);
                Message = "A password reset link has been sent to your email address.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                Message = "Failed to send reset link. Please try again later.";
            }

            return Page();
        }

        
        private void SendResetEmail(string email, string resetUrl)
        {
            //smtp details for the resetlink mail
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("votingsystem187@gmail.com", "dfgwnwtxjtokprhs"), 
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("votingsystem187@gmail.com"),
                Subject = "Password Reset Request",
                Body = $"Click the link to reset your password: <a href=\"{resetUrl}\">{resetUrl}</a>",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
            Console.WriteLine($"Password reset email sent to {email}");
        }

        public void OnGet()
        {

        }
    }
}
