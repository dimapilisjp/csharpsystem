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
        public void OnGet()
        {
            TotalElections = Database_Helper.DbHelper.GetTotalElections();
            TotalVoters = Database_Helper.DbHelper.GetTotalVoters();
            TotalVotes = Database_Helper.DbHelper.GetTotalVotes();
            PendingRegistrations = Database_Helper.DbHelper.GetPendingRegistrations();
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
