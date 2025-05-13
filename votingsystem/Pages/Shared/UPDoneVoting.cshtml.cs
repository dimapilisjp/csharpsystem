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

        //fetches the details of the user and the candidates the user voted
        public void OnGet(int electionId)
        {
            Console.WriteLine($"Username: {User.Identity.Name} ElectionID: {electionId}");
            int userId = Database_Helper.DbHelper.GetUserIdByUsername(User.Identity.Name); //DbHelper #41
            VotedCandidates = Database_Helper.DbHelper.GetUserVotedCandidates(userId, electionId); //DbHelper #43
            ElectionId = electionId;

            
            ElectionTitle = Database_Helper.DbHelper.GetElectionTitle(electionId); //DbHelper #42
        }

        //will redirect to the election total results
        public IActionResult OnPostRedirectToResults(int electionId)
        {
            Console.WriteLine($"Redirecting to live election results for ElectionId {electionId}.");
            return RedirectToPage("/Shared/UPResults", new { electionId });
        }
    }
}
