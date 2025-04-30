using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.RegistrationModel;
using static votingsystem.Pages.Shared.ADManageElectionsModel;
using static votingsystem.Pages.Shared.ADManageVotersModel;
using static votingsystem.Pages.Shared.UPDoneVotingModel;


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
            public int CandidateId { get; set; }
            public string Position { get; set; }
        }
        public List<Candidate> VotedCandidates { get; set; }
        public bool HasVoted { get; set; }
        public List<VoteReceipt> VoteReceipts { get; set; }

        public void OnGet(int electionId)
        {
            Console.WriteLine($"ElectionId: {electionId}");
            Console.WriteLine("User.Identity.Name: " + User.Identity.Name);
            Election = Database_Helper.DbHelper.GetElectionById(electionId);
            Candidates = Database_Helper.DbHelper.GetCandidatesByElectionId(electionId);

            if (Candidates == null || !Candidates.Any())
            {
                Console.WriteLine($"No candidates found for ElectionId: {electionId}");
            }         
            
        }

        public IActionResult OnPostCastVote(int electionId, Dictionary<string, int> selectedCandidates)
        {
            Console.WriteLine($"[Debug] User.Identity.Name: {User.Identity.Name}");

            try
            {
                // Validate User.Identity.Name
                if (string.IsNullOrWhiteSpace(User.Identity.Name))
                {
                    Console.WriteLine("Error: User.Identity.Name is empty. Redirecting to login.");
                    TempData["Error"] = "You need to log in to cast your vote.";
                    return RedirectToPage("/Account/Login");
                }

                // Fetch UserId
                var userId = Database_Helper.DbHelper.GetUserIdByUsername(User.Identity.Name);
                Console.WriteLine($"[Debug] Fetched UserId: {userId}");

                if (userId <= 0)
                {
                    Console.WriteLine("Error: Unable to fetch valid UserId. Redirecting to login.");
                    TempData["Error"] = "Invalid user session. Please log in again.";
                    return RedirectToPage("/Shared/Results");
                }

                Console.WriteLine($"[Debug] ElectionId: {electionId}");

                // Validate election and selectedCandidates
                if (electionId <= 0 || selectedCandidates == null || selectedCandidates.Count == 0)
                {
                    TempData["Error"] = "Invalid election or candidate selection.";
                    return RedirectToPage("/Shared/ADResults");
                }

                // Check if the user has already voted
                if (Database_Helper.DbHelper.HasUserVoted(userId, electionId))
                {
                    Console.WriteLine($"User {userId} has already voted for ElectionId {electionId}.");
                    TempData["Error"] = "You have already cast your vote for this election.";
                    return RedirectToPage("/Shared/UPDoneVoting", new { electionId });
                }

                // Record votes
                foreach (var position in selectedCandidates)
                {
                    Console.WriteLine($"Position: {position.Key}, CandidateId: {position.Value}");
                    int candidateId = position.Value;

                    var vote = new Vote
                    {
                        ElectionId = electionId,
                        UserId = userId,
                        CandidateId = candidateId,
                        Position = position.Key
                    };

                    try
                    {
                        Console.WriteLine($"Recording vote: {vote.ElectionId}, {vote.UserId}, {vote.CandidateId}, {vote.Position}");
                        Database_Helper.DbHelper.RecordVote(vote);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while recording vote for {position.Key}: {ex.Message}");
                        TempData["Error"] = "An error occurred while recording your votes.";
                    }
                }

                TempData["Success"] = "Your votes have been successfully cast!";
                Console.WriteLine($"Votes successfully recorded for UserId={userId}, ElectionId={electionId}");
                return RedirectToPage("/Shared/UPDoneVoting", new { electionId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while casting vote for UserId, ElectionId={electionId}: {ex.Message}");
                TempData["Error"] = "An unexpected error occurred while processing your votes.";
                return RedirectToPage("/Shared/UPDoneVoting", new { electionId });
            }
        }
    }
}
