using System;
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
    [Authorize(Roles="PENDING, VERIFIED, UNVERIFIED")]
    public partial class UpdateModel : PageModel
    {
        private readonly UserManager<SwishUser> _userManager;
        private readonly IProfileRepo _repo;
        private IVerificationRoleManager _verifRoleManager;
        private IProfHistoryRepo _history;

        public UpdateModel(
            UserManager<SwishUser> userManager,
            IProfileRepo repo, 
            IVerificationRoleManager verifRoleManager, 
            IProfHistoryRepo history)
        {
            _userManager = userManager;
            _repo = repo;
            _verifRoleManager = verifRoleManager;
            _history = history;
        }

        private ProfileModel Profile { get; set; }
        
        public string FullName { get; set; }
        public string Country { get; set; }
        
        public string SharedEmail { get; set; }

        [BindProperty] public InputModel Input { get; set; }
        
        [TempData] public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "New Full Name")]
            public string NewFullName { get; set; }
            
            [Required]
            [Display(Name = "New Country")]
            public string NewCountry { get; set; }
            
            [Required]
            [Display(Name = "Shared Email")]
            public string NewSharedEmail { get; set; }
        }

        private async Task LoadAsync(SwishUser user)
        {
            var id = await _userManager.GetUserIdAsync(user);
            var profile = await _repo.GetProfileByUserId(id);
            Profile = profile;
            Input = new InputModel
            {
                NewFullName = profile.Name,
                NewCountry = profile.Country,
                NewSharedEmail = profile.SharedEmail
            };
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
            await LoadAsync(user);
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            SwishUser user = await _userManager.GetUserAsync(User);
            if (user == null) { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }

            if (!ModelState.IsValid)
            { await LoadAsync(user); return Page(); }

            var currentProfile = await _repo.GetProfileByUserId(user.Id);
            var newProfile = currentProfile;
            
            if (newProfile.UserId != null)
            {
                newProfile.Name = Input.NewFullName;
                newProfile.Country = Input.NewCountry;
                newProfile.SharedEmail = Input.NewSharedEmail;
                _repo.Update(newProfile);
                await _history.Update(currentProfile);
                await _verifRoleManager.MakeUserOnlyThisRole(user, "PENDING");
                
            } else
            {
                throw new Exception("Profile does not exist yet.");
            }
            StatusMessage = "Your profile has been updated.";
            return RedirectToPage("./Update");
        }
    }
}