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

namespace SwishIdentity.Areas.Verif.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<SwishUser> _userManager;
        private readonly IPiiClaimRepo _piiclaim;
        private readonly IManagerRepo _mang;
        private readonly IProfileRepo _profile;

        public IndexModel(
            UserManager<SwishUser> userManager,
            IPiiClaimRepo piiclaim,
            IManagerRepo mang,
            IProfileRepo profile)
        {
            _userManager = userManager;
            _piiclaim = piiclaim;
            _mang = mang;
            _profile = profile;
        }

        private ProfileModel Profile { get; set; }
        public ManagerModel Manager { get; set; }
        
        private async Task LoadAsync(string mangId)
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = await _profile.GetProfileByUserId(user.Id);
            Profile = profile;
            Manager = await _mang.GetManagerByManagerId(mangId);
        }
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            string mang_id = id;
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return LocalRedirect("/Account/Login");
            }
            var manager = await _mang.GetManagerByManagerId(mang_id);
            if (manager == default)
            { return NotFound("The manager was not found"); }
            //TODO: Search for manager by email here later

            var profile = await _profile.GetProfileByUserId(user.Id);
            if (profile == default) 
            { return LocalRedirect("/Profile/Create"); }
            
            await LoadAsync(mang_id);
            //does a claim already exist?
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            SwishUser user = await _userManager.GetUserAsync(User);
            if (user == null) { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
            string mangId = HttpContext.Request.Query["id"];
            await LoadAsync(mangId);
            if (Profile == default) 
            { return LocalRedirect("/Profile/Create"); }
            if (Manager == default)
            { return LocalRedirect("/Manager/ManagerRequest"); }

            string profId = Profile.ProfileId.ToString();
            var exist = await _piiclaim.DoesClaimExist(profId, mangId);
            if (exist)
            { return LocalRedirect("/Verif/Exists"); }
            
            await _piiclaim.Post(Profile, mangId);
            return LocalRedirect("/Profile");
        }
    }
}