using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.Shared.ADManageCandidatesModel;
using static votingsystem.Pages.Shared.ADManageElectionsModel;

namespace votingsystem.Pages.Shared
{
    public class UPMyBallotsModel : PageModel
    {
        public List<Election> VotedElections { get; set; } = new List<Election>();

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

        public void OnGet()
        {
            int userId = Database_Helper.DbHelper.GetUserIdByUsername(User.Identity.Name);
            var elections = Database_Helper.DbHelper.GetUserVotedElections(userId);

            VotedElections = elections ?? new List<Election>(); // Ensure it's never null
        }

        public IActionResult OnPostShowBallot(int electionId)
        {
            return RedirectToPage("/Shared/UPDoneVoting", new { electionId });
        }
    }
}
