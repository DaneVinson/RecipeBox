using Microsoft.AspNet.Identity.EntityFramework;
using RecipeBox.Core.Interfaces;
using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.DataContext
{
    /// <summary>
    /// The primary context class for RecipeBox authentication/authorization.
    /// </summary>
    public sealed class AuthContext : IdentityDbContext<IdentityUser>, IAuthContext
    {
        public AuthContext() : base("RecipeBoxContext") { }

        /// <summary>
        /// The set of Account objects.
        /// </summary>
        public IDbSet<Account> Accounts { get; set; }

        // Not needed for AuthContext.
        public void SetPropertyModified<TEntity>(
            TEntity entity, 
            Expression<Func<TEntity, string>> expression, 
            bool isModified = true) where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}
