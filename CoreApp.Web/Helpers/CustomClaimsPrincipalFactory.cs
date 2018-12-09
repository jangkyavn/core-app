using CoreApp.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreApp.Utilities.Constants;

namespace CoreApp.Web.Helpers
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, AppRole>
    {
        private readonly UserManager<AppUser> _userManager;

        public CustomClaimsPrincipalFactory(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
        {
            _userManager = userManager;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var pricipal = await base.CreateAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            ((ClaimsIdentity)pricipal.Identity).AddClaims(new[]
            {
                new Claim(CommonConstants.UserClaims.UserId, user.Id.ToString()),
                new Claim(CommonConstants.UserClaims.UserName, user.UserName),
                new Claim(CommonConstants.UserClaims.Email, user.Email),
                new Claim(CommonConstants.UserClaims.FullName, user.FullName),
                new Claim(CommonConstants.UserClaims.Avatar, user.Avatar??string.Empty),
                new Claim(CommonConstants.UserClaims.Roles, string.Join(";", roles))
            });

            return pricipal;
        }
    }
}
