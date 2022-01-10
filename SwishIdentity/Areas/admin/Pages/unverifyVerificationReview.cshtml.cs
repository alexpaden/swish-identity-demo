using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SwishIdentity.Data;
using SwishIdentity.Data.Repositories;
using SwishIdentity.Data.Repositories.Profile;
using SwishIdentity.Data.VerificationRoleManager;

namespace SwishIdentity.Areas.Admin.Pages
{
    [Authorize(Roles="ADMIN")]
    public class UnverifyVerificationReviewModel : PageModel
    {
        private readonly UserManager<SwishUser> _userManager;
        private readonly ILogger<UnverifyVerificationReviewModel> _logger;
        private readonly IVerificationRoleManager _roleManager;
        private readonly IProfileRepo _profile;
        private readonly SignInManager<SwishUser> _signInManager;

        public UnverifyVerificationReviewModel(
            UserManager<SwishUser> userManager,
            ILogger<UnverifyVerificationReviewModel> logger, 
            IVerificationRoleManager roleManager,
            IProfileRepo profile,
            SignInManager<SwishUser> signInManager)
        {
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
            _profile = profile;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var profileId = HttpContext.Request.Query["id"];
            var verifierId = user.Id;
            var profile = await _profile.GetProfileByProfileId(profileId);
            
            var profileUser = await _userManager.FindByIdAsync(profile.UserId);
            if (await _roleManager.MakeUserOnlyThisRole(profileUser, "UNVERIFIED"))
            {
                _logger.LogInformation($"Admin with ID '{{verifierId}}' unverified '{{profileId}}'", verifierId,
                    profileId);
            }else { throw new ArgumentException(); }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage("Index");
        }
    }
}