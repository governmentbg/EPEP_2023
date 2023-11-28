using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;

namespace eCase.Common.Auth
{
    public static class AuthenticationTypes
    {
        public static readonly string Bearer = OAuthDefaults.AuthenticationType;
        public static readonly string Cookie = CookieAuthenticationDefaults.AuthenticationType;
    }
}
