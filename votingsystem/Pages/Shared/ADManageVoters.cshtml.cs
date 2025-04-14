using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace votingsystem.Pages.Shared
{
    public class ADManageVotersModel : PageModel
    {
        public IActionResult OnPostRedirectToManageVoters()
        {
            return RedirectToPage("/Shared/ADManageVoters");
        }
        public IActionResult OnPostRedirectToDashboard()
        {
            return RedirectToPage("/Shared/Dashboard");
        }

        public IActionResult OnPostRedirectToManageElections()
        {
            return RedirectToPage("/Shared/ADManageElections");
        }

        public IActionResult OnPostRedirectToResults()
        {
            return RedirectToPage("/Shared/ADResults");
        }

        // Property to hold voter data
        public List<Voter> Voters { get; set; }

        
        public void OnGet()
        {
            Voters = new List<Voter>(); 

            
            using (var connection = Database_Helper.DbHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT Id, FirstName, LastName, Email, Address, PasswordHash, Month, Day, Year, UserName, Age, Role FROM Users"; 
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Voters.Add(new Voter
                            {
                                Id = reader.GetInt32("Id"),
                                FirstName = reader.GetString("FirstName"),
                                LastName = reader.GetString("LastName"),
                                Email = reader.GetString("Email"),
                                Address = reader.GetString("Address"),
                                PasswordHash = reader.GetString("PasswordHash"),
                                Month = reader.GetString("Month"),
                                Day = reader.GetInt32("Day"),
                                Year = reader.GetInt32("Year"),
                                UserName = reader.GetString("UserName"),
                                Age = reader.GetInt32("Age"),
                                Role = reader.GetString("Role")
                            });
                        }
                    }
                }
            }
        }


        //  delete form submitted
        public IActionResult OnPostDeleteVoter(int id)
        {
            using (var connection = Database_Helper.DbHelper.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Users WHERE Id = @Id"; 
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }

            // refresh the page 
            return RedirectToPage("/Shared/ADManageVoters");
        }
    }

   
    public class Voter
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PasswordHash { get; set; }
        public string Month { get; set; }
        public int Day { get; set; }
        public int Year { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Role { get; set; }
    }
}

