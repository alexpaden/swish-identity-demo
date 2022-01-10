using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SwishIdentity.Data;
using SwishIdentity.Data.Models;
using SwishIdentity.Data.Repositories.Manager;
using SwishIdentity.Data.Repositories.PiiClaim;
using SwishIdentity.Data.Repositories.Profile;

namespace SwishIdentity.Areas.Manager.Pages
{
    [Authorize(Roles = "MANAGER, ADMIN")]
    public class IndexModel : PageModel
    {
        private readonly IManagerRepo _mang;
        private readonly IPiiClaimRepo _piiClaim;
        private readonly IProfileRepo _profile;
        private readonly UserManager<SwishUser> _userManager;

        public IndexModel(
            UserManager<SwishUser> userManager,
            IPiiClaimRepo piiClaim,
            IProfileRepo profile,
            IManagerRepo mang)
        {
            _userManager = userManager;
            _piiClaim = piiClaim;
            _profile = profile;
            _mang = mang;
        }

        public IList<ProfileModel> Profiles { get; set; }
        public object StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (!User.IsInRole("MANAGER")) return RedirectToPage("ManagerRequest");
            var manager = await _mang.GetManagerByUserId(user.Id);
            var profileList = await _piiClaim.GetAllProfilesByManagerId(manager.ManagerId);
            Profiles = profileList;
            return Page();
        }
    }
}