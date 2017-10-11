using System.Security.Claims;
using System.Threading.Tasks;
using HorseSpot.Api.Utils;
using Microsoft.Owin.Security.Facebook;

namespace HorseSpot.Api.Providers
{
    public class FacebookAuthProvider : FacebookAuthenticationProvider
    {
        public override Task Authenticated(FacebookAuthenticatedContext context)
        {
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            TryParseProperty(context, "first_name", AuthConstants.CustomClaims.FirstName);
            TryParseProperty(context, "last_name", AuthConstants.CustomClaims.LastName);
            TryParseProperty(context, "picture.data.url", AuthConstants.CustomClaims.ImageUrl);
            TryParseProperty(context, "Email", AuthConstants.CustomClaims.Email);

            return Task.FromResult<object>(null);
        }

        private void TryParseProperty(FacebookAuthenticatedContext context, string name, string targetName)
        {
            var value = context.User.SelectToken(name);

            if (value != null)
            {
                context.Identity.AddClaim(new Claim(targetName, value.ToString()));
            }
        }
    }
}