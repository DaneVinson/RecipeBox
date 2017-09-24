using RecipeBox.Core.Interfaces;
using RecipeBox.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace RecipeBox.WebApi
{
    /// <summary>
    /// RecipeBox base ApiController class.
    /// </summary>
    public abstract class BaseController : ApiController
    {
        public BaseController(IClaimsProvider claimsProvider)
        {
            this.ClaimsProvider = claimsProvider;
        }


        /// <summary>
        /// Method which checks for the existence of a  current account Id claim and returns the Id if
        /// found and null if not.
        /// </summary>
        /// <returns></returns>
        protected string GetAccountId()
        {
            var accountIdClaim = this.ClaimsProvider.GetClaim(Utility.AccountIdClaimName);
            if (accountIdClaim == null || String.IsNullOrWhiteSpace(accountIdClaim.Value)) { return null; }
            else { return accountIdClaim.Value; }
        }

        /// <summary>
        /// Get the current base URI with not path or query.
        /// </summary>
        /// <returns></returns>
        protected Uri GetBaseUri()
        {
            return new Uri(this.Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, String.Empty));
        }

        /// <summary>
        /// Return a NoContentResult (204).
        /// </summary>
        /// <returns></returns>
        protected virtual NoContentResult NoContent()
        {
            return new NoContentResult(this.Request);           
        }


        protected IClaimsProvider ClaimsProvider { get; set; }
    }
}