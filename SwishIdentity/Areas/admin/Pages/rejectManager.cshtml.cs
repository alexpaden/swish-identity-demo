using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SwishIdentity.Data;
using SwishIdentity.Data.Repositories;
using SwishIdentity.Data.Repositories.Manager;

namespace SwishIdentity.Areas.Admin.Pages
{
    [Authorize(Roles="ADMIN")]
    public class RejectManagerModel : PageModel
    {
        private readonly UserManager<SwishUser> _userManager;

        public RejectManagerModel(
            UserManager<SwishUser> userManager,
            ILogger<RejectManagerModel> logger, 
            IManagerRepo managerRepo)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return RedirectToPage("ManagerReview");
        }
    }
}