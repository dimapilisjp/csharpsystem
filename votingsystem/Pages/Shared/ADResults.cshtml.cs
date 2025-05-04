using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.Shared.ADManageElectionsModel;
using static votingsystem.Pages.Shared.UPResultsModel;

namespace votingsystem.Pages.Shared
{
    public class ADResultsModel : PageModel
    {
        public int ElectionId { get; set; }
        public List<ElectionResult> LiveResults { get; set; }

        public List<Election> AllElections { get; set; }

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

        public void OnGet(int? electionId)
        {
            Console.WriteLine($"Fetching live results for ElectionId={electionId}");

            // fetch all available elections
            AllElections = Database_Helper.DbHelper.GetAllElections();

            // if an election is selected, fetch live results
            if (electionId.HasValue)
            {
                // fetch live results for the selected election
                LiveResults = Database_Helper.DbHelper.GetLiveElectionAdminResults(electionId.Value);
            }
            else
            {
                // if no election selected, set liveresults to an empty list
                LiveResults = new List<ElectionResult>();
            }
        }

        // fetch live results for a specific election via AJAX
        public JsonResult OnGetLiveResults(int electionId)
        {
            Console.WriteLine($" AJAX call received for electionId={electionId}");
            var results = Database_Helper.DbHelper.GetLiveElectionResults(electionId);
            return new JsonResult(results);
        }


    }

}
