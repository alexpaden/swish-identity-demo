using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SwishIdentity.Data;
using SwishIdentity.Data.Repositories.Manager;
using SwishIdentity.Data.VerificationRoleManager;

namespace SwishIdentity.Areas.Admin.Pages
{
    [Authorize(Roles = "ADMIN")]
    public class ApproveManagerModel : PageModel
    {
        private readonly ILogger<ApproveManagerModel> _logger;
        private readonly IManagerRepo _mang;
        private readonly IVerificationRoleManager _roleManager;
        private readonly SignInManager<SwishUser> _signIn;
        private readonly UserManager<SwishUser> _userManager;

        public ApproveManagerModel(
            UserManager<SwishUser> userManager,
            ILogger<ApproveManagerModel> logger,
            IVerificationRoleManager roleManager,
            IManagerRepo mang,
            SignInManager<SwishUser> signIn)
        {
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
            _mang = mang;
            _signIn = signIn;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var mangUserId = HttpContext.Request.Query["id"];
            var verifierId = _userManager.GetUserId(User);
            var mangUser = await _userManager.FindByIdAsync(mangUserId);
            await _roleManager.MakeUserManager(mangUser);
            var mang = await _mang.GetManagerByUserId(mangUser.Id);
            mang.ManagerEnabled = true;
            await _mang.Update(mang);
            await _signIn.RefreshSignInAsync(user);

            return RedirectToPage("ManagerReview");
        }
    }
}