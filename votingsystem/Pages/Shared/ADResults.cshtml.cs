using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.Shared.UPResultsModel;

namespace votingsystem.Pages.Shared
{
    public class ADResultsModel : PageModel
    {
        public List<ElectionResults> ElectionData { get; set; }
        public int ElectionId { get; set; }

        public void OnGet(int electionId)
        {
            ElectionId = electionId;

            // Fetch results for the specified election ID
            ElectionData = Database_Helper.DbHelper.GetElectionResults()
                           .FindAll(e => e.ElectionId == electionId);
            ElectionData = Database_Helper.DbHelper.GetElectionResults();

            // Debugging: Ensure ElectionData is populated
            Console.WriteLine($"Fetched ElectionData for ElectionId: {ElectionId}");
            foreach (var data in ElectionData)
            {
                Console.WriteLine($"ElectionId: {data.ElectionId}, CandidateName: {data.CandidateName}, VoteCount: {data.VoteCount}");
            }
        }
    }
    
}
