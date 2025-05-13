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

        //fetches the details of the user and the candidates the user voted
        public void OnGet()
        {
            int userId = Database_Helper.DbHelper.GetUserIdByUsername(User.Identity.Name); //DbHelper #41
            var elections = Database_Helper.DbHelper.GetUserVotedElections(userId); //DbHelper #44

            VotedElections = elections ?? new List<Election>();
        }


        //will display the voted candidates
        public IActionResult OnPostShowBallot(int electionId)
        {
            return RedirectToPage("/Shared/UPDoneVoting", new { electionId });
        }
    }
}
