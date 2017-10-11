using System.Security.Claims;
using HorseSpot.Models.Models;
using Microsoft.AspNet.Identity;

namespace HorseSpot.Api.Utils
{
    public class ExternalLoginData
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string UserName { get; set; }
        public string ExternalAccessToken { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUrl { get; set; }

        public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }

            Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

            if (providerKeyClaim == null || string.IsNullOrEmpty(providerKeyClaim.Issuer) || string.IsNullOrEmpty(providerKeyClaim.Value))
            {
                return null;
            }

            if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
            {
                return null;
            }

            return new ExternalLoginData
            {
                LoginProvider = providerKeyClaim.Issuer,
                ProviderKey = providerKeyClaim.Value,
                UserName = identity.FindFirstValue(ClaimTypes.Name),
                Email = identity.FindFirstValue(ClaimTypes.Email),
                FirstName = identity.FindFirstValue(AuthConstants.CustomClaims.FirstName),
                LastName = identity.FindFirstValue(AuthConstants.CustomClaims.LastName),
                ImageUrl = identity.FindFirstValue(AuthConstants.CustomClaims.ImageUrl),
                ExternalAccessToken = identity.FindFirstValue(AuthConstants.CustomClaims.ExternalAccessToken)
            };
        }
    }
}