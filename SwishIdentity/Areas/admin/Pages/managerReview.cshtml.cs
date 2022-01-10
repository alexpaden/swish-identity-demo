using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SwishIdentity.Data;
using SwishIdentity.Data.Models;
using SwishIdentity.Data.Repositories;
using SwishIdentity.Data.Repositories.Manager;

namespace SwishIdentity.Areas.Admin.Pages
{
    [Authorize(Roles="ADMIN")]
    public class ManagerReviewModel : PageModel
    {
        private readonly UserManager<SwishUser> _userManager;
        private readonly IManagerRepo _repo;

        public ManagerReviewModel(
            UserManager<SwishUser> userManager,
            IManagerRepo repo)
        {
            _userManager = userManager;
            _repo = repo;
        }
        
        public IList<ManagerModel> Managers { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Managers = await _repo.GetAllPendingManagers();
            return Page();
        }
    }
}