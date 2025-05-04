using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static votingsystem.Pages.Shared.ADManageElectionsModel;

namespace votingsystem.Pages.Shared
{
    public class ADManageCandidatesModel : PageModel
    {
        public class Candidate
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string Address { get; set; }
            public string Position { get; set; }
            public string PartyList { get; set; }
            public int ElectionId { get; set; }
            public string PictureUrl { get; set; }
            public string Department { get; set; }
            public string Program { get; set; }
        }

        public List<Candidate> Candidates { get; set; } = new List<Candidate>();
        public List<Election> UpcomingElections { get; set; } = new List<Election>();

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

        public IActionResult OnPostCreateCandidate(Candidate candidate, IFormFile candidateImage)
        {
            if (candidateImage != null)
            {
                Console.WriteLine($"Image received: {candidateImage.FileName}, Size: {candidateImage.Length} bytes");
            }
            else
            {
                Console.WriteLine("No image received.");
                TempData["Message"] = "Candidate image is required.";
                return Page();
            }
            // to check a valid file is uploaded
            if (candidateImage != null && candidateImage.Length > 0)
            {
                // directory to save images
                var uploadsFolder = Path.Combine("wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder); //checks if the directory exists

                // save the file with its original name 
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(candidateImage.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    candidateImage.CopyTo(stream);
                }


                candidate.PictureUrl = $"/images/{fileName}";
            }
            else
            {

                TempData["Message"] = "Candidate image is required.";
                return Page();
            }


            Database_Helper.DbHelper.CreateCandidate(candidate);

            TempData["Message"] = "Candidate successfully registered.";
            return RedirectToPage("/Shared/ADManageElections");
        }

        public IActionResult OnPostDeleteCandidate(int id)
        {
            Database_Helper.DbHelper.DeleteCandidate(id);
            return RedirectToPage("/Shared/ADManageElections");
        }

        public void OnGet()
        {
            Candidates = Database_Helper.DbHelper.GetCandidates();
            UpcomingElections = Database_Helper.DbHelper.GetUpcomingElections();
        }
    }
}
