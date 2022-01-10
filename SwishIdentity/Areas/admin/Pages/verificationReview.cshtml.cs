using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SwishIdentity.Data;
using SwishIdentity.Data.Models;
using SwishIdentity.Data.Repositories;
using SwishIdentity.Data.Repositories.Profile;

namespace SwishIdentity.Areas.Admin.Pages
{
    [Authorize(Roles="ADMIN")]
    public class VerificationReviewModel : PageModel
    {
        private readonly UserManager<SwishUser> _userManager;
        private readonly IProfileRepo _profile;

        public VerificationReviewModel(
            UserManager<SwishUser> userManager,
            IProfileRepo profile)
        {
            _userManager = userManager;
            _profile = profile;
        }
        
        public ProfileModel Profile { get; set; }
        public IList<string> CurrentRoles { get; set; }
        public bool UserExist { get; set; }
        
        public string UserEmail { get; set; }

        private async Task LoadAsync(ProfileModel profile)
        {
            Profile = profile;
            var profUser = await _userManager.FindByIdAsync(profile.UserId);
            if (profUser != null) { UserExist = true; }
            CurrentRoles = await _userManager.GetRolesAsync(profUser);
            UserEmail = await _userManager.GetEmailAsync(profUser);
        }
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var profile = await _profile.GetProfileByProfileId(id);
            await LoadAsync(profile);
            return Page();
        }
    }
}