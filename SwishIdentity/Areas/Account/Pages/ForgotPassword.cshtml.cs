using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SwishIdentity.Data;
using SwishIdentity.Data.EmailService;

namespace SwishIdentity.Areas.Account.Pages
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<SwishUser> _userManager;
        private readonly ISwishEmailService _emailSender;

        public ForgotPasswordModel(UserManager<SwishUser> userManager,
        ISwishEmailService emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    //Requires confirmed email not just existing account
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Account", code },
                    protocol: Request.Scheme);
                Console.WriteLine(callbackUrl);
                await _emailSender.ResetSwishPassword(callbackUrl, user.Email);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }
            return Page();
        }
    }
}
