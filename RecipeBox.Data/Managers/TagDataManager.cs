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
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Data.Managers
{
    /// <summary>
    /// IDataManager implementation to control Tags CRUD.
    /// </summary>
    public class TagDataManager : IDataManager<Tag>
    {
        public TagDataManager(IRecipeBoxContext context)
        {
            this.Context = context;
        }


        public async Task<ICrudResult<Tag>> CreateAsync(Tag tag)
        {
            // Instantiate a default result.
            var result = new SimpleCrudResult<Tag>();

            // Add the Tag to context and save.
            if (tag != null)
            {
                this.Context.Tags.Add(tag);
                var count = await this.Context.SaveChangesAsync();
                if (count > 0)
                {
                    result.Model = tag;
                    result.Success = true;
                }
                else { result.ErrorMessage = "The Tag failed to save."; }
            }
            else { result.ErrorMessage = "Tag is null."; }

            return result;
        }

        public async Task<ICrudResult<EmptyClass>> DeleteAsync(ICrudCriteria<int> criteria)
        {
            // Initialize a result.
            var result = new SimpleCrudResult<EmptyClass>();

            // Look for the tag verifying that it belongs to the user then try to delete.
            if (criteria != null)
            {
                var tag = await this.Context.Tags.FirstOrDefaultAsync(a => a.Id == criteria.Value && a.AccountId == criteria.AccountId);
                if (tag == null) { result.ErrorMessage = "Unable to find or access the specified Tag."; }
                else
                {
                    this.Context.Tags.Remove(tag);
                    var count = await this.Context.SaveChangesAsync();
                    if (count > 0) { result.Success = true; }
                    else { result.ErrorMessage = "Delete failed to save."; }
                }
            }
            else { result.ErrorMessage = "Cannot locate a tag using a null criteria."; }

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

        public async Task<ICrudResult<Tag>> ReadAsync(ICrudCriteria<int> criteria)
        {
            // Initialize a result.
            var result = new SimpleCrudResult<Tag>();

            // Look for the tag verifying that it belongs to the user before returning it.
            if (criteria != null)
            {
                var tag = await this.Context.Tags.FirstOrDefaultAsync(a => a.Id == criteria.Value && a.AccountId == criteria.AccountId);
                if (tag == null) { result.ErrorMessage = "Unable to find or access the specified Tag."; }
                else
                {
                    result.Model = tag;
                    result.Success = true;
                }
            }
            else { result.ErrorMessage = "Cannot locate a tag using a null criteria."; }

            return result;
        }

        public async Task<ICrudResult<List<Tag>>> ReadManyAsync(ICrudCriteria<EmptyClass> criteria)
        {
            // Initialze result and verify a valid criteria.
            var result = new CollectionCrudResult<Tag>();

            // Read the tags for the user for the data source, order by descripton and return.
            if (criteria != null)
            {
                result.Model = await this.Context.Tags
                                        .Where(t => t.AccountId == criteria.AccountId)
                                        .OrderBy(t => t.Description)
                                        .ToListAsync();
                result.Success = true;
            }
            else { result.ErrorMessage = "Cannot search for tags using a null criteria."; }

            return result;
        }

        public async Task<ICrudResult<Tag>> UpdateAsync(ICrudCriteria<Tag> criteria)
        {
            // Initialize a result.
            var result = new SimpleCrudResult<Tag>();

            // Look for the tag verifying that it belongs to the user before updating only desired properties.
            if (criteria != null)
            {
                Tag tag = null;
                if (criteria.Value != null)
                {
                    tag = await this.Context.Tags
                                            .FirstOrDefaultAsync(a => a.Id == criteria.Value.Id &&
                                                                        a.AccountId == criteria.AccountId);
                }

                if (tag == null) { result.ErrorMessage = "Unable to find or access the specified Tag."; }
                else
                {
                    try
                    {
                        // Specify properties to update.
                        tag.Description = criteria.Value.Description;
                        this.Context.SetPropertyModified(tag, t => t.Description);

                        // Save and handle results
                        var saveCount = await this.Context.SaveChangesAsync();
                        if (saveCount > 0)
                        {
                            result.Model = tag;
                            result.Success = true;
                        }
                        else { result.ErrorMessage = "The Tag failed to save."; }
                    }
                    catch (DbUpdateConcurrencyException) { result.ErrorMessage = "The Tag could not be saved because it was out of date."; }
                }
            }
            else { result.ErrorMessage = "Cannot update a tag using a null criteria."; }

            return result;
        }


        private IRecipeBoxContext Context { get; set; }
    }
}
