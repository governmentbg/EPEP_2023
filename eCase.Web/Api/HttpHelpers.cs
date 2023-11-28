using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eCase.Web.Api
{
    static class HttpHelpers
    {
        /// <summary>
        /// Remove bounding quotes on a token if present
        /// NOTE: A copy of the WebApi internal System.Net.Http.FormattingUtilities.UnquoteToken
        /// </summary>
        /// <param name="token">Token to unquote.</param>
        /// <returns>Unquoted token.</returns>
        public static string UnquoteToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return token;
            }

            if (token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal) && token.Length > 1)
            {
                return token.Substring(1, token.Length - 2);
            }

            return token;
        }
    }
}