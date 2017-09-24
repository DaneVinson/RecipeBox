using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace RecipeBox.WebApi
{
    /// <summary>
    /// Abstraction for claims based operations.
    /// </summary>
    public interface IClaimsProvider
    {
        Claim GetClaim(string type);
    }
}