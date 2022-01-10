using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SwishIdentity.Data;
using SwishIdentity.Data.Repositories.Manager;

namespace SwishIdentity.Areas.Manager.Pages
{
    [Authorize(Roles = "MANAGER, ADMIN")]
    public class ShareModel : PageModel
    {
        private readonly IManagerRepo _repo;
        private readonly UserManager<SwishUser> _userManager;

        public ShareModel(
            UserManager<SwishUser> userManager,
            IManagerRepo repo)
        {
            _userManager = userManager;
            _repo = repo;
        }

        public string FullUrl { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            var mang = await _repo.GetManagerByUserId(user.Id);
            var mangId = mang.ManagerId.ToString();
            FullUrl = Url.Page(
                "/manglookup",
                pageHandler: null,
                values: new { area = "verif", id=mangId },
                protocol: "https");
            
            return Page();
        }
    }
}