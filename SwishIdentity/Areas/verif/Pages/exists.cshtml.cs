using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SwishIdentity.Areas.Verif.Pages
{
    [Authorize]
    public class ExistsModel : PageModel
    {
        public void OnGet()
        {
            
        }
    }
}