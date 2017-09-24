using Microsoft.AspNet.Identity;
using RecipeBox.Core;
using RecipeBox.Core.Interfaces;
using RecipeBox.Data.Criteria;
using RecipeBox.Model.Enumerators;
using RecipeBox.Model.Models;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace RecipeBox.WebApi.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : BaseController
    {
        public AccountController(IClaimsProvider claimsProvider, IDataManager<Account> dataManager) : base(claimsProvider)
        {
            this.DataManager = dataManager;
        }


        /// <summary>
        /// Method which activates the account associated with the input id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("activate/{id}")]
        public async Task<IHttpActionResult> ActivateAccount(string id)
        {
            Uri baseUri = GetBaseUri();
            Uri failUri = new Uri(String.Format("{0}/#/activationfail", baseUri.ToString().Trim('/')));
            Uri successUri = new Uri(String.Format("{0}/#/account", baseUri.ToString().Trim('/')));

            // Find the Account.
            var result = await this.DataManager.ReadAsync(new SimpleCriteria<int>(id));
            if (result == null || !result.Success) { return Redirect(failUri); }

            // Update Status to Active and save.
            result.Model.Status = AccountStatus.Active.ToString();
            result = await this.DataManager.UpdateAsync(new SimpleCriteria<Account>(id) { Value = result.Model });

            // Determine where to redirect and do so.
            if (result != null && result.Success) { return Redirect(successUri); }
            else { return Redirect(failUri); }
        }
        
        /// <summary>
        /// Override calls Context.Dispose if appropriate, then calls base.Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (this.DataManager != null)
            {
                this.DataManager.Dispose();
                this.DataManager = null;
            }
            base.Dispose(disposing);
        }

        [Authorize]
        [HttpGet]
        [Route("{userName}")]
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> GetAccount(string userName)
        {
            // Get the user's account Id.
            var accountIdClaim = this.ClaimsProvider.GetClaim(Utility.AccountIdClaimName);
            if (accountIdClaim == null || String.IsNullOrWhiteSpace(accountIdClaim.Value)) { return Unauthorized(); }

            // Fetch the Account.
            var criteria = new SimpleCriteria<int>(accountIdClaim.Value);
            var result = await this.DataManager.ReadAsync(criteria);
            if (result == null ||
                !result.Success ||
                result.Model == null ||
                !result.Model.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }
            else { return Ok(result.Model); }
        }

        /// <summary>
        /// Post to save a new account.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> PostAccount(Account account)
        {
            // Create the account
            var result = await this.DataManager.CreateAsync(account);
            if (result.Success) 
            {
                await SendConfirmationEmail(account.EmailAddress, account.Id);
                return Ok(account);
            }
            else { return BadRequest(result.ErrorMessage); }
        }

        /// <summary>
        /// Put to update and existing Account.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> PutAccount(Account account)
        {
            // Get the user's account Id.
            var accountIdClaim = this.ClaimsProvider.GetClaim(Utility.AccountIdClaimName);
            if (accountIdClaim == null || String.IsNullOrWhiteSpace(accountIdClaim.Value)) { return Unauthorized(); }

            // Update the account.
            var criteria = new SimpleCriteria<Account>(accountIdClaim.Value) { Value = account };
            var result = await this.DataManager.UpdateAsync(criteria);
            if (result.Success) { return Ok(account); }
            else { return BadRequest("Account was not updated."); }
        }


        /// <summary>
        /// Send an confirmation email to a new account's specified address.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task SendConfirmationEmail(string emailAddress, string id)
        {
            Uri baseUri = GetBaseUri();
            Uri confirmationUri = new Uri(String.Format("{0}/api/account/activate/{1}", baseUri.OriginalString, id));
            var htmlStringBuilder = new StringBuilder().Append("Thank you for you interest in <strong>RecipeBox</strong>.")
                                                        .Append("<br/>")
                                                        .Append("<br/>")
                                                        .AppendFormat("To activate your account simply go <a href='{0}'>here</a>. ", confirmationUri.ToString())
                                                        .Append("If your activation is successful you'll be redirected to the log in page ")
                                                        .AppendFormat("where you can enter you credentials and gain full access to <a href='{0}'>RecipeBox</a>.", baseUri.ToString());
            var textStringBuilder = new StringBuilder().Append("Thank you for you interest in RecipeBox.")
                                                        .AppendLine()
                                                        .AppendLine()
                                                        .AppendFormat("To activate your account simply go to {0}", confirmationUri.ToString())
                                                        .AppendLine()
                                                        .Append("If your activation is successful you'll be redirected to the log in page ")
                                                        .Append("where you can enter you credentials and gain full access to RecipeBox.");

            // Create the email object first, then add the properties.
            var email = new SendGridMessage(
                                new MailAddress("noreply@recipeebox.azurewebsites.net", "RecipeBox"),   // TODO: clean up very weak dependency here on recipeebox.azurewebsites.net
                                new MailAddress[] { new MailAddress(emailAddress) },
                                "RecipeBox account registration email confirmation",
                                htmlStringBuilder.ToString(),
                                textStringBuilder.ToString());

            // Create an Web transport using credentials then use it to send the email.
            var transportWeb = new Web(new NetworkCredential(Utility.SendGridUserName, Utility.SendGridPassword));
            await transportWeb.DeliverAsync(email);
        }


        private IDataManager<Account> DataManager { get; set; }
    }
}
