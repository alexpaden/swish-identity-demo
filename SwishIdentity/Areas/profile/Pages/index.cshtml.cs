using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SwishIdentity.Data;
using SwishIdentity.Data.Models;
using SwishIdentity.Data.Repositories.PiiClaim;
using SwishIdentity.Data.Repositories.Profile;
using SwishIdentity.Data.VerificationRoleManager;

namespace SwishIdentity.Areas.Profile.Pages
{
    [Authorize]
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<SwishUser> _userManager;
        private readonly IPiiClaimRepo _piiClaim;
        private IVerificationRoleManager _verifRoleManager;
        private IProfileRepo _profile;
        
        public IndexModel(
            UserManager<SwishUser> userManager,
            IPiiClaimRepo piiClaim,
            IVerificationRoleManager verifRoleManager,
            IProfileRepo profile)
        {
            _userManager = userManager;
            _piiClaim = piiClaim;
            _verifRoleManager = verifRoleManager;
            _profile = profile;
        }

        public ProfileModel Profile { get; set; }
        public string VerifStatus { get; set; }
        public IList<ManagerModel> MangList { get; set; }

        private async Task LoadAsync(SwishUser user)
        {
            var id = await _userManager.GetUserIdAsync(user);
            var profile = await _profile.GetProfileByUserId(id);
            Profile = profile;
            var mangList = await _piiClaim.GetAllManagersByProfileId(profile.ProfileId);
            MangList = mangList;
            VerifStatus = await _verifRoleManager.RolesToStringByUser(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var profile = await _profile.GetProfileByUserId(user.Id);
            if (profile == default) { return RedirectToPage("./Create"); }

            await LoadAsync(user);
            return Page();
        }
    }
}