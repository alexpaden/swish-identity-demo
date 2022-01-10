using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SwishIdentity.Data;
using SwishIdentity.Data.Models;
using SwishIdentity.Data.Repositories.Profile;

namespace SwishIdentity.Areas.Admin.Pages
{
    [Authorize(Roles = "ADMIN")]
    public class IndexModel : PageModel
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IProfileRepo _repo;
        private readonly UserManager<SwishUser> _userManager;

        public IndexModel(
            UserManager<SwishUser> userManager,
            IProfileRepo repo,
            IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _repo = repo;
            _authorizationService = authorizationService;
        }

        public IList<ProfileModel> Profiles { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("Login Required");

            Profiles = await _repo.GetAllProfiles();
            return Page();
        }
    }
}