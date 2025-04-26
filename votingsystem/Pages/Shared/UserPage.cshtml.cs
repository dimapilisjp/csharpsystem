using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.Shared.ADManageElectionsModel;

namespace votingsystem.Pages.Shared
{
    public class UserPageModel : PageModel
    {
        public List<Election> AvailableElections { get; set; }
        public List<Election> UpcomingElections { get; set; }

        public IActionResult OnPostRedirectToVote(int electionId)
        {
            return RedirectToPage("/Shared/UPVotingPage", new { electionId });
        }
       
        public void OnGet()
        {            
            AvailableElections = Database_Helper.DbHelper.GetAvailableElections();
            UpcomingElections = Database_Helper.DbHelper.GetUpcomingElections();
        }
    }
}
