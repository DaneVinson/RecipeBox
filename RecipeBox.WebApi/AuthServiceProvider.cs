using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;
using RecipeBox.Core.Interfaces;
using RecipeBox.Data.Criteria;
using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace RecipeBox.WebApi
{
    public class AuthServiceProvider : OAuthAuthorizationServerProvider
    {
        public AuthServiceProvider(IDataManager<Account> dataManager)
        {
            this.DataManager = dataManager;
        }


        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // CORS for Web API
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            string accountId = null;
            var criteria = new AuthorizationCriteria(context.UserName, context.Password);
            var result = await this.DataManager.ReadManyAsync(criteria);
            if (result.Success) { accountId = result.Model.First().Id; }

            // Set claims or set error.
            if (!String.IsNullOrWhiteSpace(accountId))
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(Utility.UserNameClaimName, context.UserName));
                identity.AddClaim(new Claim(Utility.ClientIdClaimName, context.ClientId ?? String.Empty));
                identity.AddClaim(new Claim(Utility.AccountIdClaimName, accountId ?? String.Empty));
                context.Validated(identity);
            }
            else 
            {
                context.SetError("invalid_grant", "The user name and password are incorrect.");
                context.Rejected();
            }
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }


        private IDataManager<Account> DataManager { get; set; }
    }
}