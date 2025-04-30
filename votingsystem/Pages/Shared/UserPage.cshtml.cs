using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.RegistrationModel;
using static votingsystem.Pages.Shared.ADManageElectionsModel;

namespace votingsystem.Pages.Shared
{
    public class UserPageModel : PageModel
    {
        public List<Election> AvailableElections { get; set; }
        public List<Election> UpcomingElections { get; set; }

        public IActionResult OnPostRedirectToVote(int electionId)
        {
            Console.WriteLine($"[Debug] ElectionId: {electionId}");

            // validate User.Identity.Name
            if (string.IsNullOrWhiteSpace(User.Identity.Name))
            {
                Console.WriteLine("Error: User.Identity.Name is empty. Redirecting to login.");
                TempData["Error"] = "You need to log in to access the voting page.";
                return RedirectToPage("/Account/Login");
            }

            // fetch UserId
            var userId = Database_Helper.DbHelper.GetUserIdByUsername(User.Identity.Name);
            Console.WriteLine($"[Debug] Fetched UserId: {userId}");

            if (userId <= 0)
            {
                Console.WriteLine("Error: Unable to fetch valid UserId. Redirecting to login.");
                TempData["Error"] = "Invalid user session. Please log in again.";
                return RedirectToPage("/Account/Login");
            }

            // check if the user has already voted for election
            if (Database_Helper.DbHelper.HasUserVoted(userId, electionId))
            {
                Console.WriteLine($"User {userId} has already voted for ElectionId {electionId}.");
                TempData["Error"] = "You have already cast your vote for this election.";
                return RedirectToPage("/Shared/UPDoneVoting", new { electionId });
            }

            // redirect to the voting page if the user has not voted
            Console.WriteLine($"Redirecting User {userId} to voting page for ElectionId {electionId}.");
            return RedirectToPage("/Shared/UPVotingPage", new { electionId });
        }

        public void OnGet()
        {            
            AvailableElections = Database_Helper.DbHelper.GetAvailableElections();
            UpcomingElections = Database_Helper.DbHelper.GetUpcomingElections();
        }
    }
}
