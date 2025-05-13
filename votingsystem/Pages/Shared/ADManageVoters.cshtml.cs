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
        public List<Voter> Voters { get; set; }

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
            bool isDeleted = Database_Helper.DbHelper.DeleteVoter(id); //DbHelper #12

            if (!isDeleted)
            {
                TempData["Message"] = "User cannot be deleted because they have existing votes.";
                Console.WriteLine("User cannot be deleted because they have existing votes");
            }
            else
            {
                TempData["Message"] = "User successfully deleted.";
            }

            return RedirectToPage("/Shared/ADManageVoters");
        }

        //fetches the list of voters
        public void OnGet()
        {
            Voters = Database_Helper.DbHelper.GetVoters(); //DbHelper #11
        }

        //approve the registration of the voter
        public IActionResult OnPostApproveVoter(int id)
        {
            bool isApproved = Database_Helper.DbHelper.ApproveVoter(id); //DbHelper #2
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

        //reject the registration of the voter, will be deleted
        public IActionResult OnPostRejectVoter(int id)
        {
            bool isRejected = Database_Helper.DbHelper.RejectVoter(id); //DbHelper #3
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

