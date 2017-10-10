using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Facebook;

namespace HorseSpot.Api.Providers
{
    public class FacebookAuthProvider : FacebookAuthenticationProvider
    {
        public override Task Authenticated(FacebookAuthenticatedContext context)
        {
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            TryParseProperty(context, "first_name", "FirstName");
            TryParseProperty(context, "last_name", "LastName");
            TryParseProperty(context, "picture.data.url", "Picture");
            TryParseProperty(context, "Email", "Email");

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