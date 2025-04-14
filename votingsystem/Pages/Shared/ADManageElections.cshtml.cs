using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace votingsystem.Pages.Shared
{
    public class ADManageElectionsModel : PageModel
    {
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
        public void OnGet()
        {

        }

        public class Candidate
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string Address { get; set; }
            public string Position { get; set; }
        }
    }
}
