using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SwishIdentity.Data;
using SwishIdentity.Data.Models;
using SwishIdentity.Data.Repositories.Manager;
using SwishIdentity.Data.VerificationRoleManager;

namespace SwishIdentity.Areas.Verif.Pages
{
    [AllowAnonymous]
    public class MangLookupModel : PageModel
    {
        private readonly IManagerRepo _mang;
        private readonly IVerificationRoleManager _verif;
        private readonly UserManager<SwishUser> _usr;
        
        public MangLookupModel(IManagerRepo mang,
            IVerificationRoleManager verif,
            UserManager<SwishUser> usr)
        {
            _mang = mang;
            _verif = verif;
            _usr = usr;
        }
        public string ManagerRoles { get; set; }
        public ManagerModel Manager { get; set; }
        private async Task LoadAsync(string mangId)
        {
            Manager = await _mang.GetManagerByManagerId(mangId);
            var temp = await _usr.FindByIdAsync(Manager.UserId);
            ManagerRoles = await _verif.RolesToStringByUser(temp);
        }
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAsync(id);
            return Page();
        }
    }
}