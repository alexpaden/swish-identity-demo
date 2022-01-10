using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SwishIdentity.Data;
using SwishIdentity.Data.Models;
using SwishIdentity.Data.Repositories.Profile;
using SwishIdentity.Data.Repositories.ProfileHistory;
using SwishIdentity.Data.VerificationRoleManager;

namespace SwishIdentity.Areas.Profile.Pages
{
    [Authorize(Roles="ALIEN")]
    public partial class CreateModel : PageModel
    {
        private readonly UserManager<SwishUser> _userManager;
        private readonly IProfHistoryRepo _history;
        private readonly IVerificationRoleManager _verifRoleManager;
        private readonly IProfileRepo _profileRepo;

        public CreateModel(
            UserManager<SwishUser> userManager,
            IProfHistoryRepo history, 
            IVerificationRoleManager verifRoleManager, IProfileRepo profileRepo)
        {
            _userManager = userManager;
            _history = history;
            _verifRoleManager = verifRoleManager;
            _profileRepo = profileRepo;
        }

        [BindProperty] public InputModel Input { get; set; }
        
        [TempData] public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }
            
            [Required]
            [Display(Name = "Country")]
            public string Country { get; set; }
            
            [Required]
            [Display(Name = "Shared Email")]
            public string SharedEmail { get; set; }
        }
        

        private async Task LoadAsync(SwishUser user)
        {
            var id = await _userManager.GetUserIdAsync(user); 
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            SwishUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var exist = await _profileRepo.GetProfileByUserId(user.Id);
          //  if (exist) { return RedirectToPage("./Update"); }
            await LoadAsync(user);
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            SwishUser user = await _userManager.GetUserAsync(User);
            if (user == null) { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var newProfile = new ProfileModel
            {
                User = user,
                UserId = user.Id, 
                Name = Input.FullName,
                Country = Input.Country,
                SharedEmail = Input.SharedEmail
            };
            var result = await _profileRepo.Post(newProfile);
            await _verifRoleManager.MakeUserOnlyThisRole(user, "PENDING");
            await _history.Update(newProfile);

            if (result == false) 
            {
                StatusMessage = "Unexpected error when trying to create your profile.";
                return RedirectToPage();
            }

            StatusMessage = "Your profile has been created";
            return RedirectToPage("./Update");
        }
        }
    
    
}