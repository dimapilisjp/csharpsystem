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

        //fetches the result of the election
        public void OnGet(int? electionId)
        {
            Console.WriteLine($"Fetching live results for ElectionId={electionId}");

            // fetch all available elections
            AllElections = Database_Helper.DbHelper.GetAllElections(); //DbHelper #45

            if (electionId.HasValue)
            {
                LiveResults = Database_Helper.DbHelper.GetLiveElectionAdminResults(electionId.Value); //DbHelper #46
            }
            else
            {
                LiveResults = new List<ElectionResult>();
            }
        }

        // fetch live results for a specific election
        public JsonResult OnGetLiveResults(int electionId)
        {
            Console.WriteLine($" AJAX call received for electionId={electionId}");
            var results = Database_Helper.DbHelper.GetLiveElectionResults(electionId); //DbHelper #47
            return new JsonResult(results);
        }


    }

}
