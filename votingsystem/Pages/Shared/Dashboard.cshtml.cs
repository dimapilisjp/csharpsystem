using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace votingsystem.Pages.Shared
{
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
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

        public IActionResult OnPostRedirectToResults()
        {
            return RedirectToPage("/Shared/ADResults");
        }

    }
}
