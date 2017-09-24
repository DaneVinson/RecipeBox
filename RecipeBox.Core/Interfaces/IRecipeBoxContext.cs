using RecipeBox.Model;
using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RecipeBox.Core.Interfaces
{
    /// <summary>
    /// Interface with defines the primary context for RecipeBox.
    /// </summary>
    public interface IRecipeBoxContext : IContext
    {
        /// <summary>
        /// The set of Account objects.
        /// </summary>
        IDbSet<Account> Accounts { get; set; }

        /// <summary>
        /// The set of Ingredident objects.
        /// </summary>
        IDbSet<Ingredient> Ingredients { get; set; }

        /// <summary>
        /// The set of Recipe objects.
        /// </summary>
        IDbSet<Recipe> Recipes { get; set; }

        /// <summary>
        /// The set of Tag objects.
        /// </summary>
        IDbSet<Tag> Tags { get; set; }
    }
}
