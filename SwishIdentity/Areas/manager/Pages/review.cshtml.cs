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
    public class ReviewModel : PageModel
    {
        private readonly IManagerRepo _mang;
        private readonly IPiiClaimRepo _piiClaim;
        private readonly IProfileRepo _prof;
        private readonly UserManager<SwishUser> _userManager;
        private readonly UserManager<SwishUser> _usr;

        public ReviewModel(
            UserManager<SwishUser> userManager,
            IProfileRepo prof,
            IPiiClaimRepo piiclaim,
            IManagerRepo mang,
            UserManager<SwishUser> usr)
        {
            _userManager = userManager;
            _prof = prof;
            _piiClaim = piiclaim;
            _mang = mang;
            _usr = usr;
        }

        public ProfileModel Profile { get; set; }
        public string Roles { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            var profile = await _prof.GetProfileByProfileId(id);
            if (profile == default) return NotFound($"No Profile '{id}'.");
            Profile = profile;

            var manager = await _mang.GetManagerByUserId(user.Id);
            if (manager == default) return NotFound($"No Manager '{id}'.");

            //Does a PII claim between user + manager exist
            var isAllowed = await _piiClaim.DoesClaimExist(profile.ProfileId.ToString(),
                manager.ManagerId.ToString());
            if (!isAllowed) return RedirectToPage("/Index");

            var profileUser = await _usr.FindByIdAsync(profile.UserId);
            var roles = await _usr.GetRolesAsync(profileUser);
            Roles = string.Join(",", roles);

            return Page();
        }
    }
}