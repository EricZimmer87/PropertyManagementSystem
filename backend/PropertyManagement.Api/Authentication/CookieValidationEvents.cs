using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using PropertyManagement.Api.Models;

namespace PropertyManagement.Api.Authentication
{
    public class CookieValidationEvents : CookieAuthenticationEvents
    {
        private readonly UserManager<AppUser> _userManager;

        public CookieValidationEvents(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            if (context.Principal == null)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                return;
            }

            var user = await _userManager.GetUserAsync(context.Principal);

            if (user == null || !user.IsActive)
            {
                context.RejectPrincipal();

                await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            }

            await base.ValidatePrincipal(context);
        }
    }
}
