using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.RegistrationModel;
using static votingsystem.Pages.Shared.ADManageElectionsModel;

namespace votingsystem.Pages.Shared
{
    public class UPResultsModel : PageModel
    {
        public int ElectionId { get; set; }
        public List<ElectionResult> LiveResults { get; set; }
        public List<Election> AllElections { get; set; }

        public class ElectionResult
        {
            public int ElectionId { get; set; }
            public int CandidateId { get; set; }
            public string CandidateName { get; set; }
            public int VoteCount { get; set; }
            public string Position { get; set; }
        }

        public IActionResult OnPostRedirectToBallot()
        {
            return RedirectToPage("/Shared/UPMyBallots");
        }

        public IActionResult OnPostRedirectToHome()
        {
            return RedirectToPage("/Shared/UserPage");
        }

        public IActionResult OnPostRedirectToResults(int electionId)
        {
            return RedirectToPage("/Shared/UPResults", new { electionId });
        }
        public IActionResult OnPostLogout()
        {
            HttpContext.SignOutAsync();
            return RedirectToPage("/Index");
        }

        // Fetch all elections
        public void OnGet(int? electionId)
        {
            Console.WriteLine($"Fetching live results for ElectionId={electionId}");

            // Fetch all available elections
            AllElections = Database_Helper.DbHelper.GetAllElections();

            // If an election is selected, fetch live results
            if (electionId.HasValue)
            {
                LiveResults = Database_Helper.DbHelper.GetLiveElectionResults(electionId.Value);
            }
            else
            {
                LiveResults = new List<ElectionResult>();
            }
        }

        // Fetch live results for a specific election via AJAX
        public JsonResult OnGetLiveResults(int electionId)
        {
            Console.WriteLine($" AJAX call received for electionId={electionId}");
            var results = Database_Helper.DbHelper.GetLiveElectionResults(electionId);
            return new JsonResult(results);
        }


    }

}
