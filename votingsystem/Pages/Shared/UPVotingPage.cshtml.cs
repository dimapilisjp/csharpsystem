using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.Shared.ADManageElectionsModel;

namespace votingsystem.Pages.Shared
{
    public class VotingPageModel : PageModel
    {
        public Election Election { get; set; }
        public List<Candidate> Candidates { get; set; }

        public void OnGet(int electionId)
        {
            Election = Database_Helper.DbHelper.GetElectionById(electionId);
            Candidates = Database_Helper.DbHelper.GetCandidatesByElectionId(electionId);

            if (Candidates == null || !Candidates.Any())
            {
                Console.WriteLine($"No candidates found for ElectionId: {electionId}");
            }
        }

        public IActionResult OnPostCastVote(int electionId, int candidateId)
        {
            var userId = Database_Helper.DbHelper.GetUserIdByUsername(User.Identity.Name);

            if (Database_Helper.DbHelper.HasUserVoted(userId, electionId))
            {
                TempData["Error"] = "You have already cast your vote for this election.";
                return RedirectToPage("/Vote", new { electionId });
            }

            Database_Helper.DbHelper.RecordVote(userId, electionId, candidateId);

            TempData["Success"] = "Your vote has been successfully cast!";
            return RedirectToPage("/Thanks");
        }
    }
}
