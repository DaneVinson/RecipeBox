using RecipeBox.Core.Interfaces;
using RecipeBox.Model;
using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.DataContext
{
    /// <summary>
    /// The primary context class for RecipeBox.
    /// </summary>
    public class RecipeBoxContext : DbContext, IRecipeBoxContext
    {
        /// <summary>
        /// Default constructor disables Entity Framework proxy generation.
        /// </summary>
        public RecipeBoxContext()
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Set the specified property on the input entity to the requested modified state (default true).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="entity"></param>
        /// <param name="isModified">Defaults true</param>
        public void SetPropertyModified<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> expression, 
            bool isModified = true) 
            where TEntity : class
        {
            this.Entry(entity).Property(expression).IsModified = isModified;
        }

        /// <summary>
        /// The set of Account objects.
        /// </summary>
        public IDbSet<Account> Accounts { get; set; }

        /// <summary>
        /// The set of Ingredident objects.
        /// </summary>
        public IDbSet<Ingredient> Ingredients { get; set; }

        /// <summary>
        /// The set of Recipe objects.
        /// </summary>
        public IDbSet<Recipe> Recipes { get; set; }

        /// <summary>
        /// The set of Tag objects.
        /// </summary>
        public IDbSet<Tag> Tags { get; set; }
    }
}
