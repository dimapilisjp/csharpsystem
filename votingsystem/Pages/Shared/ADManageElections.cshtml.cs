using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace votingsystem.Pages.Shared
{
    public class ADManageElectionsModel : PageModel
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

        public IActionResult OnPostCreateCandidate(Candidate candidate)
        {
            Database_Helper.DbHelper.CreateCandidate(candidate);
            return RedirectToPage("/Shared/ADManageElections");
        }

        public IActionResult OnPostDeleteCandidate(int id)
        {
            Database_Helper.DbHelper.DeleteCandidate(id);
            return RedirectToPage("/Shared/ADManageElections");
        }

        public List<Candidate> Candidates { get; set; } = new List<Candidate>();
        public void OnGet()
        {
            //Candidates = Database_Helper.DbHelper.GetCandidates();

            using (var connection = Database_Helper.DbHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT Id, Name, Age, Address, Position FROM Candidates";

                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Candidates.Add(new Candidate
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Age = reader.GetInt32("Age"),
                                Address = reader.GetString("Address"),
                                Position = reader.GetString("Position")
                            });
                        }
                    }
                }
            }

        }

        public class Candidate
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string Address { get; set; }
            public string Position { get; set; }
        }
    }
}
