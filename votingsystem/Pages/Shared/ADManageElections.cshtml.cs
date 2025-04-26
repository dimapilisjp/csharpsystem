using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace votingsystem.Pages.Shared
{
    public class ADManageElectionsModel : PageModel
    {
        public class Election
        {
            public int ElectionId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string Status { get; set; }
        }

        public class Candidate
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string Address { get; set; }
            public string Position { get; set; }
            public string PartyList { get; set; }
            public int ElectionId { get; set; }
        }
        public List<Candidate> Candidates { get; set; } = new List<Candidate>();
        public List<Election> Elections { get; set; } = new List<Election>();
        public Election EditElection { get; set; }
        public List<Election> UpcomingElections { get; set; } = new List<Election>();

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
            Console.WriteLine($"ElectionId received from form: {candidate.ElectionId}");
            Candidates = Database_Helper.DbHelper.GetCandidates();
            // if candidate already exists
            if (Database_Helper.DbHelper.IsCandidateNameRegistered(candidate.Name))
            {
                TempData["Message"] = "The name is already registered.";
                return Page();
            }
          
            Database_Helper.DbHelper.CreateCandidate(candidate);
            TempData["Message"] = "Candidate successfully registered.";
            return RedirectToPage("/Shared/ADManageElections");           
        }    
           

        public IActionResult OnPostDeleteCandidate(int id)
        {
            Database_Helper.DbHelper.DeleteCandidate(id);
            return RedirectToPage("/Shared/ADManageElections");
        }
        public IActionResult OnPostCreateElection(Election election)
        {
            Console.WriteLine($"Received Title: {election.Title}, Start Time: {election.StartTime}, End Time: {election.EndTime}");

            if (election.EndTime <= election.StartTime)
            {
                TempData["Message"] = "End time must be after the start time.";
                return Page();
            }

            if (Database_Helper.DbHelper.IsElectionTitleRegistered(election.Title))
            {
                TempData["Message"] = "An election with this title is already registered.";
                return Page();
            }

            Database_Helper.DbHelper.CreateElection(election);
            TempData["Message"] = "Election successfully created.";
            return RedirectToPage("/Shared/ADManageElections");

        }
        public IActionResult OnPostCreateOrEditElection(Election election)
        {
            if (election.EndTime <= election.StartTime)
            {
                TempData["Message"] = "End time must be after the start time.";
                return Page();
            }

            if (election.ElectionId > 0)
            {
                // update existing election
                bool result = Database_Helper.DbHelper.UpdateElection(election);
                TempData["Message"] = result ? "Election updated successfully." : "Error updating the election.";
            }
            else
            {
                // new election
                if (Database_Helper.DbHelper.IsElectionTitleRegistered(election.Title))
                {
                    TempData["Message"] = "An election with this title already exists.";
                    return Page();
                }

                Database_Helper.DbHelper.CreateElection(election);
                TempData["Message"] = "Election successfully created.";
            }

            return RedirectToPage("/Shared/ADManageElections");
        }



        public IActionResult OnPostDeleteElection(int id)
        {
            Database_Helper.DbHelper.DeleteElection(id);
            return RedirectToPage("/Shared/ADManageElections");
        }

        public void OnGet()
        {
            Candidates = Database_Helper.DbHelper.GetCandidates();
            Elections = Database_Helper.DbHelper.GetElections();
            UpcomingElections = Database_Helper.DbHelper.GetUpcomingElections();
        }

        public void OnGetEditElection(int id)
        {
            EditElection = Database_Helper.DbHelper.GetElectionById(id);

            if (EditElection != null)
            {
                Console.WriteLine($"Editing Election: Title={EditElection.Title}, StartTime={EditElection.StartTime}");
            }
            else
            {
                Console.WriteLine("Election not found.");
            }
        }

    }
}
