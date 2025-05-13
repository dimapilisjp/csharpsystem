using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.Shared.ADManageElectionsModel;

namespace votingsystem.Pages.Shared
{
    public class ADVotesRecordModel : PageModel
    {
        public class VoteHistory
        {
            public int VoteId { get; set; }
            public int UserId { get; set; }
            public int ElectionId { get; set; }
            public string UserName { get; set; }
            public int CandidateId { get; set; }
            public string Position { get; set; }
            public string CandidateName { get; set; }
        }

        public class VoteDetails
        {
            public List<VoteSelection> Selections { get; set; } = new List<VoteSelection>();
        }

        public class VoteSelection
        {
            public int CandidateId { get; set; }
            public string CandidateName { get; set; }
            public string Position { get; set; }
        }

    
        public List<VoteHistory> VoteHistories { get; set; }
        public VoteDetails SelectedVoteDetails { get; set; }

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

        public void OnGet()
        {
            Console.WriteLine("Fetching vote history...");
            VoteHistories = Database_Helper.DbHelper.GetVoteHistories(); //DbHelper #15
        }

        //fetches the details of the user and the electionid the user voted in
        public IActionResult OnGetVoteDetails(int userId, int electionId)
        {
            Console.WriteLine($"Fetching vote receipt for User ID: {userId}, Election ID: {electionId}");
            var voteDetails = Database_Helper.DbHelper.GetVoteDetails(userId, electionId); //DbHelper #16

            if (voteDetails == null || !voteDetails.Selections.Any())
            {
                Console.WriteLine("No ballot data found!");
                return NotFound();
            }

            Console.WriteLine($"Vote receipt generated: {voteDetails.Selections.Count} selections");
            return new JsonResult(voteDetails);
        }
        

    }
}
