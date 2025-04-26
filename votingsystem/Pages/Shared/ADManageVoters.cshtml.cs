using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace votingsystem.Pages.Shared
{
    public class ADManageVotersModel : PageModel
    {
        public class Voter
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string PasswordHash { get; set; }
            public string Month { get; set; }
            public int Day { get; set; }
            public int Year { get; set; }
            public string UserName { get; set; }
            public int Age { get; set; }
            public string Role { get; set; }
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
        public IActionResult OnPostDeleteVoter(int id)
        {
            Database_Helper.DbHelper.DeleteVoter(id);
            return RedirectToPage("/Shared/ADManageVoters");
        }
        public List<Voter> Voters { get; set; }

        public void OnGet()
        {
            Voters = Database_Helper.DbHelper.GetVoters();
        }



    }
}

