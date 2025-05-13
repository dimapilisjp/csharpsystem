using Microsoft.AspNetCore.Authentication;
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
            public string Department { get; set; }
            public string Program { get; set; }
        }

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

        public IActionResult OnPostRedirectToVotesRecord()
        {
            return RedirectToPage("/Shared/ADVotesRecord");
        }

        public IActionResult OnPostRedirectToResults()
        {
            return RedirectToPage("/Shared/ADResults");
        }

        public IActionResult OnPostRedirectToManageCandidates()
        {
            return RedirectToPage("/Shared/ADManageCandidates");
        }

        public IActionResult OnPostRedirectToElectionsData()
        {
            return RedirectToPage("/Shared/ADElectionsData");
        }


        public IActionResult OnPostLogOut()
        {
            HttpContext.SignOutAsync();
            return RedirectToPage("/Index");
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

            if (string.IsNullOrEmpty(election.Department))
            {
                throw new Exception("Department is required.");
            }

            if (string.IsNullOrEmpty(election.Program))
            {
                election.Program = null; 
            }


            Database_Helper.DbHelper.CreateElection(election); //DbHelper #18
            TempData["Message"] = "Election successfully created.";
            return RedirectToPage("/Shared/ADManageElections");

        }
        public IActionResult OnPostCreateOrEditElection(Election election, int id)
        {

            if (election.EndTime <= election.StartTime)
            {
                TempData["Message"] = "End time must be after the start time.";
                return Page();
            }

            if (election.ElectionId > 0)
            {
                // update existing election
                bool result = Database_Helper.DbHelper.UpdateElection(election); //DbHelper #22
                TempData["Message"] = result ? "Election updated successfully." : "Error updating the election.";
            }
            else
            {
                // new election
                if (Database_Helper.DbHelper.IsElectionTitleRegistered(election.Title)) //DbHelper #20
                {
                    EditElection = Database_Helper.DbHelper.GetElectionById(id); //DbHelper #21
                    TempData["Message"] = "An election with this title already exists.";
                    return RedirectToPage("/Shared/ADManageElections");
                }

                Database_Helper.DbHelper.CreateElection(election); //DbHelper #18
                TempData["Message"] = "Election successfully created.";
            }

            EditElection = Database_Helper.DbHelper.GetElectionById(id); //DbHelper #21
            return RedirectToPage("/Shared/ADManageElections");
        }


        //delete the election
        public IActionResult OnPostDeleteElection(int id)
        {
            Database_Helper.DbHelper.DeleteElection(id); //DbHelper #23
            return RedirectToPage("/Shared/ADManageElections");
        }

        //fetches the elections
        public void OnGet(int id)
        {
            Elections = Database_Helper.DbHelper.GetElections(); //DbHelper #19
            EditElection = Database_Helper.DbHelper.GetElectionById(id); //DbHelper #21
        }

        //edit the election
        public void OnGetEditElection(int id)
        {
            EditElection = Database_Helper.DbHelper.GetElectionById(id); //DbHelper #21

            if (EditElection != null)
            {
                Console.WriteLine($"Editing Election: Title={EditElection.Title}, StartTime={EditElection.StartTime}, Department={EditElection.Department}");
            }
            else
            {
                Console.WriteLine("Election not found.");
            }
        }

    }
}
