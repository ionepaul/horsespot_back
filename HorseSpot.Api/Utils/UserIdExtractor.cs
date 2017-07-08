using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace HorseSpot.Api.Utils
{
    public class UserIdExtractor
    {
        /// <summary>
        /// Gets the user id from request
        /// </summary>
        /// <param name="request">Http Request Message</param>
        /// <returns></returns>
        public static string GetUserIdFromRequest(HttpRequestMessage request)
        {
            ClaimsPrincipal principal = request.GetRequestContext().Principal as ClaimsPrincipal;
            var userId = principal.Claims.Where(c => c.Type == "UserId").Single().Value;

            return userId;
        }
    }
}