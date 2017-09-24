using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Core.Interfaces
{
    /// <summary>
    /// Interface with defines the primary context for authentication and authorization.
    /// </summary>
    public interface IAuthContext : IContext
    {
        /// <summary>
        /// The set of Account objects.
        /// </summary>
        IDbSet<Account> Accounts { get; set; }
    }
}
