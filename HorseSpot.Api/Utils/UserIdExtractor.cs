using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace HorseSpot.Api.Utils
{
    public class UserIdExtractor
    {
        public static string GetUserIdFromRequest(HttpRequestMessage request)
        {
            ClaimsPrincipal principal = request.GetRequestContext().Principal as ClaimsPrincipal;
            var userId = principal.Claims.Where(c => c.Type == "UserId").Single().Value;

            return userId;
        }
    }
}