using System.Security.Claims;
using AtlasTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AtlasTracker.Services.Factories;

public class BTUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<BTUser, IdentityRole>
{
    public BTUserClaimsPrincipalFactory(UserManager<BTUser> userManager,
        RoleManager<IdentityRole> roleManager, 
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(BTUser user)
    {

        ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("Company Id", user.CompanyId.ToString()));
        return identity;
    }
}