using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.Shared.ADManageElectionsModel;

namespace votingsystem.Pages.Shared
{
    public class ADElectionsDataModel : PageModel
    {
        public class VoterStats
        {
            public string Department { get; set; }
            public string Program { get; set; }
            public int TotalVoters { get; set; }
            public int UsersVoted { get; set; }
            public double VotePercentage { get; set; }
        }

        public class VoteComparison
        {
            public string Position { get; set; }
            public string CandidateName { get; set; }
            public int VoteCount { get; set; }
            public double VotePercentage { get; set; }
        }

        public class VoteDistribution
        {
            public string Position { get; set; }
            public string CandidateName { get; set; }
            public int VoteCount { get; set; }
            public double VotePercentage { get; set; }
        }

        public List<Election> AvailableElections { get; set; }
        public List<VoterStats> VoterStatistics { get; set; } = new List<VoterStats>();
        public List<VoteComparison> VoteCompare { get; set; } = new List<VoteComparison>();

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
            //will display the available elections
            AvailableElections = Database_Helper.DbHelper.GetAvailableElections();
        }



        public IActionResult OnGetElectionData(int electionId)
        {
            // fetch the voter turnout by department/program
            var voterStats = Database_Helper.DbHelper.GetVoterTurnout(electionId);

            // fetch the vote count and percentage for candidates
            var voteCompare = Database_Helper.DbHelper.GetVoteDistribution(electionId);

            return new JsonResult(new { voterStatistics = voterStats, voteCompare = voteCompare });
        }

    }
}
