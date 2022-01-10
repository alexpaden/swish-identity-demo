using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using SwishIdentity.Data;
using SwishIdentity.Data.Repositories.Profile;
using SwishIdentity.Data.Repositories.ProfileHistory;
using SwishIdentity.Data.VerificationRoleManager;

namespace SwishIdentity.Areas.Profile.Pages
{
    [Authorize(Roles="PENDING, VERIFIED, UNVERIFIED")]
    public class HistoryModel : PageModel
    {
        private readonly UserManager<SwishUser> _userManager;
        private readonly SignInManager<SwishUser> _signInManager;
        private readonly IProfileRepo _repo;
        private IProfHistoryRepo _history;

        public HistoryModel(
            UserManager<SwishUser> userManager,
            SignInManager<SwishUser> signInManager,
            IProfileRepo repo, IVerificationRoleManager verifRoleManager, IProfHistoryRepo history)
        {
            _userManager = userManager;
            _repo = repo;
            _history = history;
        }
        
        public JArray ProfileHistory { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
            var profile = await _repo.GetProfileByUserId(user.Id);
            string historyString = await _history.GetHistoryJsonByProfile(profile);
            string jsonString = String.Concat("[", historyString, "]");
            var obj = JArray.Parse(jsonString);
            ProfileHistory = obj;
            return Page();
        }
    }
}