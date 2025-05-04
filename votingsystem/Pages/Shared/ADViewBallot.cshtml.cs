using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.Shared.ADManageCandidatesModel;

namespace votingsystem.Pages.Shared
{
    public class ADViewBallotModel : PageModel
    {
        public List<Candidate> VotedCandidates { get; set; }
        public string ElectionTitle { get; set; }

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

        public void OnGet(int userId, int electionId)
        {
            Console.WriteLine($"Loading vote receipt for User ID: {userId}, Election ID: {electionId}");

            VotedCandidates = Database_Helper.DbHelper.GetUserVotedCandidates(userId, electionId);
            ElectionTitle = Database_Helper.DbHelper.GetElectionTitle(electionId);
        }

    }
}
