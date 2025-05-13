using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace votingsystem.Pages.Shared
{
    public class DashboardModel : PageModel
    {
        public int TotalElections { get; set; }
        public int TotalVoters { get; set; }
        public int TotalVotes { get; set; }
        public int PendingRegistrations { get; set; }

        //fetches the number of total number of elections, voters, votes, pending registrations 
        public void OnGet()
        {
            TotalElections = Database_Helper.DbHelper.GetTotalElections(); //DbHelper #28
            TotalVoters = Database_Helper.DbHelper.GetTotalVoters(); //DbHelper #29
            TotalVotes = Database_Helper.DbHelper.GetTotalVotes(); //DbHelper #30
            PendingRegistrations = Database_Helper.DbHelper.GetPendingRegistrations(); //DbHelper #31
        }

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

    }
}
