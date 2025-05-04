using System.ComponentModel.DataAnnotations;
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
            [Required(ErrorMessage = "First Name is required")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last Name is required")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string Email { get; set; }

            public string Address { get; set; }
            public string Month { get; set; }
            [Range(1, 31, ErrorMessage = "Day must be between 1 and 31")]
            public int Day { get; set; }
            [Range(1900, 2100, ErrorMessage = "Year must be valid")]
            public int Year { get; set; }

            [Required(ErrorMessage = "Username is required")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
            public string PasswordHash { get; set; }

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

            // Calculate age based on birthdate
            int monthNumber = GetMonthNumber(user.Month);
            DateTime birthDate = new DateTime(user.Year, monthNumber, user.Day);
            int age = CalculateAge(birthDate);
            user.Age = age;

            Console.WriteLine($"Calculated Age: {user.Age}");
            if (age < 18)
            {
                TempData["Message"] = "You must be at least 18 years old to register.";
                Console.WriteLine("User is underaged. Registration blocked.");
                return Page(); // Stay on the registration page
            }

            // Handle photo upload
            if (photoFile != null)
            {
                Console.WriteLine($"Image received: {photoFile.FileName}, Size: {photoFile.Length} bytes");
            }
            else
            {
                Console.WriteLine("No photo file received.");
                TempData["Message"] = "Photo file (School ID / Certificate of Registration) is required.";
                return Page(); // Stay on the registration page
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
                return Page(); // Stay on the registration page
            }

            // Hash the password using BCrypt
            Console.WriteLine("Hashing password...");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            Console.WriteLine($"Hashed Password: {user.PasswordHash}");

            // Register user in the database
            bool isRegistered = votingsystem.Database_Helper.DbHelper.RegisterUser(user);

            if (!isRegistered)
            {
                Console.WriteLine("Registration failed - Username or Email might already exist.");
                TempData["Message"] = "Registration failed. Username or email may already be in use.";
                return Page(); // Stay on the registration page
            }

            TempData["Message"] = "User successfully registered. Please wait for admin approval.";
            return RedirectToPage("/Registration"); // Redirect to registration success page
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
