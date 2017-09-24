using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace RecipeBox.WebApi
{
    /// <summary>
    /// Class used to expose claims.
    /// </summary>
    public class ClaimsProvider : IClaimsProvider
    {
        /// <summary>
        /// Get the requested Claim.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Claim GetClaim(string type)
        {
            if (ClaimsPrincipal.Current == null || ClaimsPrincipal.Current.Claims == null) { return null; }
            else { return ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == type); }
        }
    }
}