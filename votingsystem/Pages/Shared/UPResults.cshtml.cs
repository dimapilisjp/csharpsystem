using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.RegistrationModel;
using static votingsystem.Pages.Shared.ADManageElectionsModel;

namespace votingsystem.Pages.Shared
{
    public class UPResultsModel : PageModel
    {
        public int ElectionId { get; set; }
        public List<ElectionResults> ElectionData { get; set; }
        public List<ElectionResults> VotedCandidates { get; set; }

        public class ElectionResults
        {
            public int ElectionId { get; set; }
            public int CandidateId { get; set; }
            public string CandidateName { get; set; }
            public int VoteCount { get; set; }
            
        }

        public void OnGet(int electionId)
        {
            ElectionId = electionId;

            ElectionData = Database_Helper.DbHelper.GetElectionResults()
                          .FindAll(e => e.ElectionId == electionId);
            ElectionData = Database_Helper.DbHelper.GetElectionResults();

            Console.WriteLine("Fetched ElectionData:");
            foreach (var data in ElectionData)
            {
                Console.WriteLine($"ElectionId: {data.ElectionId}, CandidateName: {data.CandidateName}, VoteCount: {data.VoteCount}");
            }
        }
    }
}
