using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using votingsystem.Database_Helper;


namespace votingsystem.Pages
{
    public class RegistrationModel : PageModel
    {
        public IActionResult OnPostRedirectToLogin()
        {
            return RedirectToPage("/Index"); // redirect to login page
        }

        public class User
        {
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
        }

        public class DbHelper
        {
            private readonly string _connectionString;

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        
        }


        private readonly DbHelper _DbHelper;

        //public RegistrationModel()
        //{
        //    string connectionString = "YourConnectionStringHere"; 
        //    _DbHelper = new DbHelper(connectionString);
        //}

        [BindProperty]
        public User Input { get; set; }

        public IActionResult OnPost()
        {
            Console.WriteLine("Form submitted. Inspecting ModelState...");

            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}, Error: {error.Value.Errors.FirstOrDefault()?.ErrorMessage}");
                }
                ViewData["ErrorMessage"] = "Please fill in all the required fields.";
                return Page(); 
            }

            int monthNumber = GetMonthNumber(Input.Month); 
            DateTime birthDate = new DateTime(Input.Year, monthNumber, Input.Day);
            int age = CalculateAge(birthDate); 
            Input.Age = age; 
            Console.WriteLine($"Calculated Age: {Input.Age}");

            if (age < 18)
            {
                ViewData["ErrorMessage"] = "You must be at least 18 years old to register.";
                Console.WriteLine("User is underaged. Registration blocked.");
                return Page(); 
            }


            Console.WriteLine($"Received Data - FirstName: {Input.FirstName}, LastName: {Input.LastName}, Email: {Input.Email}, Password: {Input.PasswordHash}");

            Console.WriteLine("Hashing password...");
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Input.PasswordHash);
            Input.PasswordHash = hashedPassword; 
            Console.WriteLine($"Hashed Password: {Input.PasswordHash}");

            
            Console.WriteLine("Calling RegisterUser method...");
            bool isRegistered = votingsystem.Database_Helper.DbHelper.RegisterUser(Input);

            
            if (!isRegistered)
            {
                Console.WriteLine("Registration failed - Username or Email might already exist.");
                ViewData["ErrorMessage"] = "Registration failed. Username or email may already be in use.";
                return Page(); 
            }

            Console.WriteLine($"FirstName: {Input.FirstName}");
            Console.WriteLine($"LastName: {Input.LastName}");
            Console.WriteLine($"Email: {Input.Email}");
            Console.WriteLine($"UserName: {Input.UserName}");
            Console.WriteLine($"PasswordHash: {Input.PasswordHash}");
            Console.WriteLine("Registration succeeded.");

            ViewData["SuccessMessage"] = "You have successfully registered! Please proceed to the login page.";
            return Page(); 
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
