using Microsoft.AspNet.Identity;
using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Core.Interfaces
{
    /// <summary>
    /// Implementation of Microsoft.AspNet.Identity.IUserStore where TUser is Account.
    /// </summary>
    public interface IAccountStore : IUserStore<Account>, IUserEmailStore<Account>
    {
        /// <summary>
        /// Authenticate account credentials returning the account's Id if authenticated.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<string> AuthenticateAccount(string userName, string password);


        /// <summary>
        /// The IAuthContext object to use.
        /// </summary>
        IAuthContext Context { get; set; }
    }
}
