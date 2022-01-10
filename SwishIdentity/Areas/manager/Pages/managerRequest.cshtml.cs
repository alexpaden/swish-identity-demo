using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SwishIdentity.Data;
using SwishIdentity.Data.Models;
using SwishIdentity.Data.Repositories;
using SwishIdentity.Data.Repositories.Manager;

namespace SwishIdentity.Areas.Manager.Pages
{
    [Authorize]
    public partial class ManagerRequestModel : PageModel
    {
        private readonly UserManager<SwishUser> _userManager;
        private readonly IManagerRepo _managerRepo;

        public ManagerRequestModel(
            UserManager<SwishUser> userManager,
            IManagerRepo managerRepo)
        {
            _userManager = userManager;
            _managerRepo = managerRepo;
        }
        
        public class InputModel
        {
            [Display(Name = "New Reason")]
            public string NewReason { get; set; }
            
            [Display(Name = "New Business Name")]
            public string NewBusinessName { get; set; }
            
            [Display(Name = "New Business Email")]
            public string NewBusinessEmail { get; set; }
        }
        [BindProperty] public InputModel Input { get; set; }

        public string Reason { get; set; }
        
        public string BusinessName { get; set; }
        
        public string BusinessEmail { get; set; }

        public ManagerModel Manager { get; set; }
        
        public bool Existed { get; set; }
        
        public IList<string> ProfileVerifRoles { get; set; }
        
        private async Task LoadAsync(SwishUser user)
        {
            Manager = await _managerRepo.GetManagerByUserId(user.Id);
            Existed = true;
            if(Manager == default) {
                var manager = new ManagerModel{
                    User = user,
                    Reason = "",
                    BusinessName = "",
                    BusinessEmail = "",
                    ManagerEnabled = false
                    };
                Manager = manager;
                Existed = false;
            }

            ProfileVerifRoles = await _userManager.GetRolesAsync(user);
            
            Input = new InputModel
            {
                NewReason = Manager.Reason,
                NewBusinessName = Manager.BusinessName,
                NewBusinessEmail = Manager.BusinessEmail
            };
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            SwishUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            await LoadAsync(user);
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            SwishUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (!ModelState.IsValid) { await LoadAsync(user); return Page(); }

            var manager = await _managerRepo.GetManagerByUserId(user.Id);
            if (manager == default)
            {
                var newManager = new ManagerModel
                {
                    UserId = user.Id, 
                    Reason = Input.NewReason, 
                    ManagerEnabled = false,
                    BusinessName = Input.NewBusinessName,
                    BusinessEmail = Input.NewBusinessEmail
                };
                await _managerRepo.Post(newManager);
                return RedirectToPage();
            }
            manager.Reason = Input.NewReason;
            manager.BusinessName = Input.NewBusinessName;
            manager.BusinessEmail = Input.NewBusinessEmail;
            await _managerRepo.Update(manager);
            await LoadAsync(user);
            return RedirectToPage();
        }
    }
}