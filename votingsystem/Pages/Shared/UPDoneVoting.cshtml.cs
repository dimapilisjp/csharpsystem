using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.Shared.ADManageElectionsModel;
using static votingsystem.Pages.Shared.ADManageCandidatesModel;
using Microsoft.AspNetCore.Authentication;

namespace votingsystem.Pages.Shared
{
    public class UPDoneVotingModel : PageModel
    {

        public List<Candidate> VotedCandidates { get; set; }
        public string ElectionTitle { get; set; }

        public class VoteReceipt
        {
            public string CandidateName { get; set; }
            public string Position { get; set; }
        }
        public int ElectionId { get; set; }

        public IActionResult OnPostRedirectToBallot()
        {
            return RedirectToPage("/Shared/UPMyBallots");
        }

        public IActionResult OnPostRedirectToHome()
        {
            return RedirectToPage("/Shared/UserPage");
        }
        public IActionResult OnPostLogout()
        {
            HttpContext.SignOutAsync();
            return RedirectToPage("/Index");
        }

        public void OnGet(int electionId)
        {
            int userId = Database_Helper.DbHelper.GetUserIdByUsername(User.Identity.Name); // Assuming username is stored in User.Identity
            VotedCandidates = Database_Helper.DbHelper.GetUserVotedCandidates(userId, electionId);
            ElectionId = electionId;

            // You can also fetch the election title if needed
            ElectionTitle = Database_Helper.DbHelper.GetElectionTitle(electionId);
        }

        public IActionResult OnPostRedirectToResults(int electionId)
        {
            Console.WriteLine($"Redirecting to live election results for ElectionId {electionId}.");
            return RedirectToPage("/Shared/UPResults", new { electionId });
        }
    }
}
