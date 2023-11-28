using Epep.Core.Constants;
using System.Security.Claims;

namespace Epep.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserType(this ClaimsPrincipal User)
        {

            int result = 0;

            if (User != null && User.Claims != null && User.Claims.Count() > 0)
            {
                var subClaim = User.Claims
                    .FirstOrDefault(c => c.Type == CustomClaimTypes.UserType);

                if (subClaim != null)
                {
                    result = int.Parse(subClaim.Value);
                }
            }

            return result;

        }
    }
}
