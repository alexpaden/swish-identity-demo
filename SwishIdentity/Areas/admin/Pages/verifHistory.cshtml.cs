using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using SwishIdentity.Data;
using SwishIdentity.Data.Models;
using SwishIdentity.Data.Repositories;
using SwishIdentity.Data.Repositories.Profile;
using SwishIdentity.Data.Repositories.ProfileHistory;

namespace SwishIdentity.Areas.Admin.Pages
{
    [Authorize(Roles="ADMIN")]
    public class VerifHistoryModel : PageModel
    {
        private readonly UserManager<SwishUser> _userManager;
        private readonly IProfileRepo _profile;
        private readonly IProfHistoryRepo _history;

        public VerifHistoryModel(
            UserManager<SwishUser> userManager,
            IProfileRepo profile, 
            RoleManager<IdentityRole> roleManager, 
            IProfHistoryRepo history)
        {
            _userManager = userManager;
            _profile = profile;
            _history = history;
        }
        
        public JArray ProfileHistory { get; set; }
        public ProfileModel Profile { get; set; }
        public bool UserExist { get; set; }

        private async Task LoadAsync(ProfileModel profile)
        {
            Profile = profile;
            var isUser = await _userManager.FindByIdAsync(profile.UserId);
            if (isUser != null) { UserExist = true; }
            var historyString =await _history.GetHistoryJsonByProfile(profile);
            string jsonString = String.Concat("[", historyString, "]");
            var obj = JArray.Parse(jsonString);
            ProfileHistory = obj;
        }
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var profile = await _profile.GetProfileByProfileId(id);
            await LoadAsync(profile);
            return Page();
        }
    }
}