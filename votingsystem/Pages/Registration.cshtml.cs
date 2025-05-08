using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using votingsystem.Database_Helper;
using static votingsystem.Pages.RegistrationModel;


namespace votingsystem.Pages
{
    public class RegistrationModel : PageModel
    {

        public class User
        {
            public int Id { get; set; }
            [Required(ErrorMessage = "First Name required")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last Name required")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Email required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string Email { get; set; }

            public string Address { get; set; }
            public string Month { get; set; }
            
            public int Day { get; set; }
            
            public int Year { get; set; }

            [Required(ErrorMessage = "Username required")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Password required")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
            public string PasswordHash { get; set; }

            [Required(ErrorMessage = "Confirm Password required")]
            [Compare("PasswordHash", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; }

            public int Age { get; set; }
            public string Department { get; set; }
            public string Program { get; set; }
            public string PhotoPath { get; set; }
        }

        public class DbHelper
        {
            private readonly string _connectionString;

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        
        }
        [BindProperty]
        public User Input { get; set; }
        [BindProperty]
        public string Message { get; set; }



        private readonly DbHelper _DbHelper;

        public IActionResult OnPostRedirectToLogin()
        {
            return RedirectToPage("/Index"); 
        }

        public IActionResult OnPostRegisterUser(User user, IFormFile photoFile)
        {
            Console.WriteLine($"Form submitted: FirstName={user.FirstName}, LastName={user.LastName}, Email={user.Email} Department={user.Department}");

            int monthNumber = GetMonthNumber(Input.Month);
            DateTime birthDate = new DateTime(Input.Year, monthNumber, Input.Day);
            int age = CalculateAge(birthDate);
            user.Age = age;

            Console.WriteLine($"Calculated Age: {Input.Age}");
            if (age < 18)
            {
                TempData["Message"] = "You must be at least 18 years old to register.";
                Console.WriteLine("User is underaged. Registration blocked.");
                return Page();
            }

            if (user.PasswordHash != user.ConfirmPassword)
            {
                Console.WriteLine("Password and Confirm Password do not match");
                ModelState.AddModelError(string.Empty, "Passwords do not match");
                TempData["Message"] = "Passwords do not match";
                return Page();
            }

            // handle photo upload
            if (photoFile != null)
            {
                Console.WriteLine($"Image received: {photoFile.FileName}, Size: {photoFile.Length} bytes");
            }
            else
            {
                Console.WriteLine("No photo file received.");
                TempData["Message"] = "Photo file (School ID / Certificate of Registration) is required.";
                return Page(); 
            }

            if (photoFile != null && photoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    photoFile.CopyTo(stream);
                }

                user.PhotoPath = $"/images/{fileName}";
            }
            else
            {
                TempData["Message"] = "Photo file is required.";
                return Page(); 
            }

            // hash password using BCrypt
            Console.WriteLine("Hashing password...");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            Console.WriteLine($"Hashed Password: {user.PasswordHash}");

            // generate OTP
            string otp = Database_Helper.DbHelper.GenerateOTP();
            Database_Helper.DbHelper.StoreOTP(user.Email, otp);
            Database_Helper.DbHelper.SendEmailOTP(user.Email, otp);

            // store user data in tempdata for OTP confirmation
            TempData["User"] = JsonSerializer.Serialize(user);
            TempData["Email"] = user.Email;

            Console.WriteLine($"OTP sent to {user.Email}");

            return RedirectToPage("/Shared/OTPPage"); // OTP input page
        }




        private int GetMonthNumber(string month)
        {
            return DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
        }

        private int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }



    }
}
