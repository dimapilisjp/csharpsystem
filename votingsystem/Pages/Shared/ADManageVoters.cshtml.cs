using Microsoft.AspNetCore.Authentication;
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
            public string Department { get; set; }
            public string Program {  get; set; }
            public bool IsApproved { get; set; }
            public string PhotoPath { get; set; }

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

        public IActionResult OnPostApproveVoter(int id)
        {
            bool isApproved = Database_Helper.DbHelper.ApproveVoter(id);
            if (isApproved)
            {
                TempData["Message"] = "Voter approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve voter. Please try again.";
            }
            return RedirectToPage("/Shared/ADManageVoters"); 
        }


        public IActionResult OnPostRejectVoter(int id)
        {
            bool isRejected = Database_Helper.DbHelper.RejectVoter(id);
            if (isRejected)
            {
                TempData["Message"] = "Voter rejected and removed.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject voter. Please try again.";
            }
            return RedirectToPage("/Shared/ADManageVoters");
        }
    }
}

