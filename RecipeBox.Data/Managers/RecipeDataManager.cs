using RecipeBox.Core;
using RecipeBox.Core.Interfaces;
using RecipeBox.Data.Criteria;
using RecipeBox.Data.Results;
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
    /// IDataManager implementation to control Recipe CRUD.
    /// </summary>
    public class RecipeDataManager : IDataManager<Recipe>
    {
        public RecipeDataManager(IRecipeBoxContext context, IBlobStorage blobStore)
        {
            this.BlobStore = blobStore;
            this.Context = context;
        }


        public async Task<ICrudResult<Recipe>> CreateAsync(Recipe recipe)
        {
            // Instantiate a default result.
            var result = new SimpleCrudResult<Recipe>();

            // Add the Recipe to context and save.
            if (recipe != null)
            {
                recipe = this.Context.Recipes.Add(recipe);

                // Recipe-to-Tag is many-to-many so tags must be attached individually.
                recipe.Tags.ToList().ForEach(t => this.Context.Tags.Attach(t));

                var count = await this.Context.SaveChangesAsync();
                if (count > 0)
                {
                    result.Model = recipe;
                    result.Success = true;
                }
                else { result.ErrorMessage = "The Recipe failed to save."; }
            }
            else { result.ErrorMessage = "Recipe is null."; }

            return result;
        }

        public async Task<ICrudResult<EmptyClass>> DeleteAsync(ICrudCriteria<int> criteria)
        {
            // Initialize a result.
            var result = new SimpleCrudResult<EmptyClass>();

            // Look for the recipe verifying that it belongs to the user then try to delete.
            var recipe = await this.Context.Recipes.FirstOrDefaultAsync(a => a.Id == criteria.Value && a.AccountId == criteria.AccountId);
            if (recipe == null) { result.ErrorMessage = "Unable to find or access the specified Recipe."; }
            else
            {
                this.Context.Recipes.Remove(recipe);
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
            }
        }

        public async Task<ICrudResult<Recipe>> ReadAsync(ICrudCriteria<int> criteria)
        {
            // Initialize a result.
            var result = new SimpleCrudResult<Recipe>();

            // Look for the recipe verifying that it belongs to the user before returning it.
            var recipe = await this.Context.Recipes
                                            .Include(r => r.Ingredients)
                                            .Include(r => r.Tags)
                                            .FirstOrDefaultAsync(a => a.Id == criteria.Value && a.AccountId == criteria.AccountId);
            if (recipe == null) { result.ErrorMessage = "Unable to find or access the specified Recipe."; }
            else
            {
                recipe.Tags = recipe.Tags == null ? new Tag[0] : recipe.Tags.OrderBy(t => t.Description).ToArray();
                result.Model = recipe;
                result.Success = true; 
            }

            return result;
        }

        public async Task<ICrudResult<List<Recipe>>> ReadManyAsync(ICrudCriteria<EmptyClass> criteria)
        {
            // Initialze result and verify a valid criteria.
            var result = new CollectionCrudResult<Recipe>();

            // Start building the query.
            var query = this.Context.Recipes.Include(r => r.Tags).Where(r => r.AccountId == criteria.AccountId);

            // Add RecipeCriteria to the query if applicable.
            var recipeCriteria = criteria as RecipesCriteria;
            if (recipeCriteria != null)
            {
                if (!String.IsNullOrWhiteSpace(recipeCriteria.NameFilter))
                {
                    query = query.Where(r => r.Name.StartsWith(recipeCriteria.NameFilter));
                }
                if (recipeCriteria.TagId > 0)
                {
                    query = query.Where(r => r.Tags.Any(t => t.Id == recipeCriteria.TagId));
                }
            }

            // Evaluate the query and return results.
            query = query.OrderBy(r => r.Name);
            result.Model = await query.ToListAsync();
            result.Success = true;
            return result;
        }

        public async Task<ICrudResult<Recipe>> UpdateAsync(ICrudCriteria<Recipe> criteria)
        {
            // Initialize a result.
            var result = new SimpleCrudResult<Recipe>() { Model = criteria.Value };

            // Look for the recipe verifying that it belongs to the user before updating only desired properties.
            try
            {
                Recipe dbRecipe = null;
                if (criteria.Value != null)
                {
                    // Get the Recipe and it's Ingredients, Steps and Tags
                    dbRecipe = await this.Context.Recipes
                                                .Include(r => r.Ingredients)
                                                .Include(r => r.Tags)
                                                .FirstOrDefaultAsync(r => r.Id == criteria.Value.Id && r.AccountId == criteria.AccountId);
                }

                if (dbRecipe == null)
                {
                    result.ErrorMessage = "Unable to find or access the specified Recipe.";
                    return result;
                }

                // Update primitive properties on Recipe
                dbRecipe.Description = criteria.Value.Description;
                dbRecipe.Directions = criteria.Value.Directions;
                dbRecipe.Name = criteria.Value.Name;
                dbRecipe.Notes = criteria.Value.Notes;
                dbRecipe.PreparationMinutes = criteria.Value.PreparationMinutes;
                dbRecipe.Servings = criteria.Value.Servings;

                // If the image file changed delete any current image file.
                if (!String.IsNullOrWhiteSpace(dbRecipe.ImageFileName) &&
                    !dbRecipe.ImageFileName.Equals(criteria.Value.ImageFileName, StringComparison.OrdinalIgnoreCase))
                {
                    await this.BlobStore.DeleteAsync(dbRecipe.ImageFileName);
                }
                dbRecipe.ImageFileName = criteria.Value.ImageFileName;

                // Remove missing children
                dbRecipe.Ingredients
                        .Where(d => !criteria.Value.Ingredients.Any(i => i.Id == d.Id))
                        .Select(d => d.Id).ToList()
                        .ToList()
                        .ForEach(i =>
                        {
                            var ingredient = dbRecipe.Ingredients.Single(n => n.Id == i);
                            dbRecipe.Ingredients.Remove(ingredient);
                            Context.Entry(ingredient).State = EntityState.Deleted;
                        });
                dbRecipe.Tags
                        .Where(d => !criteria.Value.Tags.Any(t => t.Id == d.Id))
                        .Select(d => d.Id).ToList()
                        .ToList()
                        .ForEach(i =>
                        {
                            dbRecipe.Tags.Remove(dbRecipe.Tags.Single(t => t.Id == i));
                        });

                // Add or update Ingredients
                foreach (var ingredient in criteria.Value.Ingredients)
                {
                    var dbIngredient = dbRecipe.Ingredients.FirstOrDefault(i => i.Id == ingredient.Id);
                    if (dbIngredient == null)
                    {
                        dbIngredient = new Ingredient() { RecipeId = dbRecipe.Id };
                        dbRecipe.Ingredients.Add(dbIngredient);
                    }
                    dbIngredient.Description = ingredient.Description;
                    dbIngredient.Quantity = ingredient.Quantity;
                    dbIngredient.Units = ingredient.Units;
                }

                // Add new Tags
                foreach (var tag in criteria.Value.Tags.Where(t => !dbRecipe.Tags.Any(d => d.Id == t.Id)))
                {
                    this.Context.Tags.Attach(tag);
                    dbRecipe.Tags.Add(tag);
                }

                // Save and handle results
                var saveCount = await this.Context.SaveChangesAsync();
                if (saveCount > 0)
                {
                    result.Model = dbRecipe;
                    result.Success = true;
                }
                else { result.ErrorMessage = "The Recipe failed to save."; }
            }
            catch (DbUpdateConcurrencyException) { result.ErrorMessage = "The Recipe could not be saved because it was out of date."; }
            return result;
        }


        private readonly IBlobStorage BlobStore;
        private readonly IRecipeBoxContext Context;
    }
}
