using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.RegistrationModel;
using static votingsystem.Pages.Shared.ADManageElectionsModel;
using static votingsystem.Pages.Shared.ADManageCandidatesModel;
using static votingsystem.Pages.Shared.ADManageVotersModel;
using static votingsystem.Pages.Shared.UPDoneVotingModel;
using Microsoft.AspNetCore.Authentication;


namespace votingsystem.Pages.Shared
{
    public class VotingPageModel : PageModel
    {
        public Election Election { get; set; }
        public List<Candidate> Candidates { get; set; }

        [BindProperty(SupportsGet = true)]
        public int ElectionId { get; set; }
        public int UserId { get; set; }

        public class Vote
        {
            public int UserId { get; set; }
            public int ElectionId { get; set; }
            public int? PresidentCandidateId { get; set; }
            public int? VicePresidentCandidateId { get; set; }
            public int? SecretaryCandidateId { get; set; }
            public int? TreasurerCandidateId { get; set; }
            public int? AuditorCandidateId { get; set; }
            public int? PROCandidateId { get; set; }
        }

        public List<Candidate> VotedCandidates { get; set; }
        public bool HasVoted { get; set; }
        public List<VoteReceipt> VoteReceipts { get; set; }

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

        //will fetch the electionid based on the details of the user, like the department and program
        public void OnGet(int electionId)
        {
            Console.WriteLine($"ElectionId: {electionId}");
            Console.WriteLine("User.Identity.Name: " + User.Identity.Name);
            Election = Database_Helper.DbHelper.GetElectionById(electionId); //DbHelper #21
            Candidates = Database_Helper.DbHelper.GetCandidatesByElectionId(electionId); //DbHelper #37

            if (Candidates == null || !Candidates.Any())
            {
                Console.WriteLine($"No candidates found for ElectionId: {electionId}");
            }
        }

        //cast the vote
        public IActionResult OnPostCastVote(int electionId, Dictionary<string, int?> selectedCandidates)
        {
            Console.WriteLine($"[Debug] User.Identity.Name: {User.Identity.Name}");

            try
            {
                if (string.IsNullOrWhiteSpace(User.Identity.Name))
                {
                    Console.WriteLine("Error: User.Identity.Name is empty. Redirecting to login.");
                    TempData["Error"] = "You need to log in to cast your vote.";
                    return RedirectToPage("/Account/Login");
                }

                var userId = Database_Helper.DbHelper.GetUserIdByUsername(User.Identity.Name); //DbHelper #41
                Console.WriteLine($"[Debug] Fetched UserId: {userId}");

                if (userId <= 0)
                {
                    Console.WriteLine("Error: Unable to fetch valid UserId. Redirecting to login.");
                    TempData["Error"] = "Invalid user session. Please log in again.";
                    return RedirectToPage("/Shared/Results");
                }

                Console.WriteLine($"[Debug] ElectionId: {electionId}");

                if (electionId <= 0 || selectedCandidates == null || selectedCandidates.Count == 0)
                {
                    TempData["Error"] = "Invalid election or candidate selection.";
                    return RedirectToPage("/Shared/ADResults");
                }

                if (Database_Helper.DbHelper.HasUserVoted(userId, electionId)) //DbHelper #38
                {
                    Console.WriteLine($"User {userId} has already voted for ElectionId {electionId}.");
                    TempData["Error"] = "You have already cast your vote for this election.";
                    return RedirectToPage("/Shared/UPDoneVoting", new { electionId });
                }

                //in order to allow null votes
                var vote = new Vote
                {
                    UserId = userId,
                    ElectionId = electionId,
                    PresidentCandidateId = selectedCandidates.ContainsKey("President") ? selectedCandidates["President"] : null,
                    VicePresidentCandidateId = selectedCandidates.ContainsKey("Vice President") ? selectedCandidates["Vice President"] : null,
                    SecretaryCandidateId = selectedCandidates.ContainsKey("Secretary") ? selectedCandidates["Secretary"] : null,
                    TreasurerCandidateId = selectedCandidates.ContainsKey("Treasurer") ? selectedCandidates["Treasurer"] : null,
                    AuditorCandidateId = selectedCandidates.ContainsKey("Auditor") ? selectedCandidates["Auditor"] : null,
                    PROCandidateId = selectedCandidates.ContainsKey("PRO") ? selectedCandidates["PRO"] : null
                };

                try
                {
                    Console.WriteLine($"Recording vote: {vote.UserId}, ElectionId={vote.ElectionId}");

                    Database_Helper.DbHelper.RecordVote(vote); //DbHelper #39
                    TempData["Success"] = "Your votes have been successfully cast!";

                    Console.WriteLine($"Votes successfully recorded for UserId={userId}, ElectionId={electionId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while recording vote: {ex.Message}");
                    TempData["Error"] = "An error occurred while recording your votes.";
                }

                return RedirectToPage("/Shared/UPDoneVoting", new { electionId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while casting vote for ElectionId={electionId}: {ex.Message}");
                TempData["Error"] = "An unexpected error occurred while processing your votes.";
                return RedirectToPage("/Shared/UPDoneVoting", new { electionId });
            }
        }
    }

}
