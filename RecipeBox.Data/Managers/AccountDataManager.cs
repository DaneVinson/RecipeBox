using RecipeBox.Core;
using RecipeBox.Core.Interfaces;
using RecipeBox.Data.Criteria;
using RecipeBox.Data.Results;
using RecipeBox.Model.Enumerators;
using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Data.Managers
{
    /// <summary>
    /// IDataManager implementation to control Accounts CRUD.
    /// </summary>
    public class AccountDataManager : IDataManager<Account>
    {
        public AccountDataManager(IRecipeBoxContext context)
        {
            this.Context = context;
        }


        public async Task<ICrudResult<Account>> CreateAsync(Account account)
        {
            // Instantiate a default result.
            var result = new SimpleCrudResult<Account>();

            // Add the Account to context and save.
            if (account != null)
            {
                // Verify no existing accounts with the same user name or email address.
                var existingAccount = await this.Context.Accounts
                                                        .FirstOrDefaultAsync(a => a.UserName == account.UserName ||
                                                                                    a.EmailAddress == account.EmailAddress);
                if (existingAccount == null)
                {
                    // Set status as unconfirmed, encrypt the password, save and handle results.
                    account.Id = Guid.NewGuid().ToString();
                    account.Status = AccountStatus.Unconfirmed.ToString();
                    account.Salt = CoreUtility.GenerateSalt();
                    account.Password = CoreUtility.EncryptPassword(account.Password, account.Salt);
                    this.Context.Accounts.Add(account);
                    if (await this.Context.SaveChangesAsync() > 0)
                    {
                        result.Model = account;
                        result.Success = true;
                    }
                    else { result.ErrorMessage = "The Account failed to save."; }
                }
                else if (existingAccount.UserName == account.UserName) { result.ErrorMessage = String.Format("The user name {0} is already in use.", account.UserName); }
                else { result.ErrorMessage = String.Format("The email address {0} is already in use.", account.EmailAddress); }
            }
            else { result.ErrorMessage = "Account is null."; }

            return result;
        }

        /// <summary>
        /// Move the account into deleted status.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public async Task<ICrudResult<EmptyClass>> DeleteAsync(ICrudCriteria<int> criteria)
        {
            // Initialize a result.
            var result = new SimpleCrudResult<EmptyClass>();

            // Find the account and attempt to delete it.
            var account = await this.Context.Accounts.FirstOrDefaultAsync(a => a.Id == criteria.AccountId);
            if (account == null) { result.ErrorMessage = "Unable to find or access the specified Account."; }
            else
            {
                account.Status = AccountStatus.Deleted.ToString();
                var count = await this.Context.SaveChangesAsync();
                if (count > 0) { result.Success = true; }
                else { result.ErrorMessage = "Delete failed to save."; }
            }

            return result;
        }

        public void Dispose()
        {
            if (this.Context != null)
            {
                this.Context.Dispose();
                this.Context = null;
            }
        }

        public async Task<ICrudResult<Account>> ReadAsync(ICrudCriteria<int> criteria)
        {
            // Initialize a result.
            var result = new SimpleCrudResult<Account>();

            // Look for the account.
            var account = await this.Context.Accounts.FirstOrDefaultAsync(a => a.Id == criteria.AccountId);
            if (account == null) { result.ErrorMessage = "Unable to find or access the specified Account."; }
            else
            {
                result.Model = account;
                result.Success = true;
            }

            return result;
        }

        public async Task<ICrudResult<List<Account>>> ReadManyAsync(ICrudCriteria<EmptyClass> criteria)
        {
            // Initialze result and verify a valid criteria.
            var result = new CollectionCrudResult<Account>();

            // If the read request is authorization attempt to authorize otherwise throw.
            var authorizationCriteria = criteria as AuthorizationCriteria;
            if (authorizationCriteria != null)
            {
                var activeStatus = AccountStatus.Active.ToString();
                var account = await this.Context.Accounts
                                                .Where(a => a.UserName == authorizationCriteria.UserName &&
                                                            a.Status == activeStatus)
                                                .FirstOrDefaultAsync();
                if (account != null && account.Password == CoreUtility.EncryptPassword(authorizationCriteria.Password, account.Salt))
                {
                    result.Model = new List<Account>() { account };
                    result.Success = true;
                }
                else { result.ErrorMessage = "Authorization failed."; }
            }
            else { throw new InvalidOperationException(); }

            return result;
        }

        public async Task<ICrudResult<Account>> UpdateAsync(ICrudCriteria<Account> criteria)
        {
            // Initialize a result.
            var result = new SimpleCrudResult<Account>();

            // Fetch the account.
            Account account = null;
            if (criteria.Value != null)
            {
                account = await this.Context.Accounts
                                            .FirstOrDefaultAsync(a => a.Id == criteria.AccountId);
            }

            if (account == null) { result.ErrorMessage = "Unable to find the specified account."; }
            else
            {
                try
                {
                    // Update only specific properties.
                    account.EmailAddress = criteria.Value.EmailAddress;
                    account.UserName = criteria.Value.UserName;
                    this.Context.SetPropertyModified(account, a => a.UserName);
                    if (account.Password != criteria.Value.Password) { account.Password = CoreUtility.EncryptPassword(criteria.Value.Password, account.Salt); }

                    // Save and handle results
                    var saveCount = await this.Context.SaveChangesAsync();
                    if (saveCount > 0)
                    {
                        result.Model = account;
                        result.Success = true;
                    }
                    else { result.ErrorMessage = "The Account failed to save."; }
                }
                catch (DbUpdateConcurrencyException) { result.ErrorMessage = "The Account could not be saved because it was out of date."; }
            }

            return result;
        }


        private IRecipeBoxContext Context { get; set; }
    }
}
