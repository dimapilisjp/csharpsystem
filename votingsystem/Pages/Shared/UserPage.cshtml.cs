using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.RegistrationModel;
using static votingsystem.Pages.Shared.ADManageElectionsModel;

namespace votingsystem.Pages.Shared
{
    public class UserPageModel : PageModel
    {
        public string UserName { get; set; }
        public List<Election> AvailableElections { get; set; }
        public List<Election> UpcomingElections { get; set; }

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


        public IActionResult OnPostRedirectToVote(int electionId)
        {
            Console.WriteLine($"[Debug] ElectionId: {electionId}");

            // validate User.Identity.Name
            if (string.IsNullOrWhiteSpace(User.Identity.Name))
            {
                Console.WriteLine("Error: User.Identity.Name is empty. Redirecting to login.");
                TempData["Error"] = "You need to log in to access the voting page.";
                return RedirectToPage("/Index");
            }

            // fetch id
            var userId = Database_Helper.DbHelper.GetUserIdByUsername(User.Identity.Name); //DbHelper #41
            Console.WriteLine($"[Debug] Fetched UserId: {userId}");

            if (userId <= 0)
            {
                Console.WriteLine("Error: Unable to fetch valid UserId. Redirecting to login.");
                TempData["Error"] = "Invalid user session. Please log in again.";
                return RedirectToPage("/Index");
            }

            // check if user has already voted for election 
            if (Database_Helper.DbHelper.HasUserVoted(userId, electionId)) //DbHelper #38
            {
                Console.WriteLine($"User {userId} has already voted for ElectionId {electionId}.");
                TempData["Error"] = "You have already cast your vote for this election.";
                return RedirectToPage("/Shared/UPDoneVoting", new { electionId });
            }

            // redirect to voting page if the user has not voted
            Console.WriteLine($"Redirecting User {userId} to voting page for ElectionId {electionId}.");
            return RedirectToPage("/Shared/UPVotingPage", new { electionId });
        }

        public void OnGet()
        {
            Console.WriteLine($"Fetching user elections for User: {User.Identity.Name}");

            // validate User.Identity.Name
            if (string.IsNullOrWhiteSpace(User.Identity.Name))
            {
                Console.WriteLine("Error: User.Identity.Name is empty. Redirecting to login.");
                TempData["Error"] = "You need to log in to access your elections.";
                RedirectToPage("/Index");
                return;
            }

            // fetch user details
            var user = Database_Helper.DbHelper.GetUserDetailsByUsername(User.Identity.Name); //DbHelper #32
            if (user == null)
            {
                Console.WriteLine("Error: Unable to fetch user details. Redirecting to login.");
                TempData["Error"] = "Invalid user session. Please log in again.";
                RedirectToPage("/Index");
                return;
            }
            Console.WriteLine($"User Details: Department={user.Department}, Program={user.Program}");

            AvailableElections = Database_Helper.DbHelper.GetAvailableElections(user.Department, user.Program); //DbHelper #33
            UpcomingElections = Database_Helper.DbHelper.GetUpcomingElectionsByUser(user.Department, user.Program); //DbHelper #36


            Console.WriteLine($"Fetched {AvailableElections.Count} available elections and {UpcomingElections.Count} upcoming elections.");
        }
    }

}
