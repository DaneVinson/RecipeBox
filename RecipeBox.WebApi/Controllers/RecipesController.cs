using Newtonsoft.Json;
using RecipeBox.Core;
using RecipeBox.Core.Interfaces;
using RecipeBox.Data.Criteria;
using RecipeBox.Model.Models;
using RecipeBox.WebApi.Criteria;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace RecipeBox.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/recipes")]
    public class RecipesController : BaseController
    {
        public RecipesController(IClaimsProvider claimsProvider, IDataManager<Recipe> dataManager, IBlobStorage blobStorage) : base(claimsProvider) 
        {
            this.BlobStore = blobStorage;
            this.DataManager = dataManager;
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteRecipe(int id)
        {
            // Get the user's account Id.
            var accountIdClaim = this.ClaimsProvider.GetClaim(Utility.AccountIdClaimName);
            if (accountIdClaim == null || String.IsNullOrWhiteSpace(accountIdClaim.Value)) { return Unauthorized(); }

            // Create the criteria object and delete.
            var criteria = new SimpleCriteria<int>(accountIdClaim.Value) { Value = id };
            var result = await this.DataManager.DeleteAsync(criteria);
            if (result.Success) { return NoContent(); }
            else { return NotFound(); }
        }

        [HttpGet]
        [Route("filtered")]
        public async Task<IEnumerable<Recipe>> GetRecipes([FromUri]RecipesSearchCriteria criteria)
        {
            // Get the user's account Id.
            var accountIdClaim = this.ClaimsProvider.GetClaim(Utility.AccountIdClaimName);
            if (accountIdClaim == null || String.IsNullOrWhiteSpace(accountIdClaim.Value)) { return new Recipe[0]; }

            // Get the data.
            var recipeCriteria = new RecipesCriteria(accountIdClaim.Value) { NameFilter = criteria.NameFilter, TagId = criteria.TagId };
            var result = await this.DataManager.ReadManyAsync(recipeCriteria);

            // Results
            if (result != null && result.Success) { return result.Model; }
            else { return new Recipe[0]; }
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(Recipe))]
        public async Task<IHttpActionResult> GetRecipe(int id)
        {
            // Get the user's account Id.
            var accountIdClaim = this.ClaimsProvider.GetClaim(Utility.AccountIdClaimName);
            if (accountIdClaim == null || String.IsNullOrWhiteSpace(accountIdClaim.Value)) { return Unauthorized(); }

            // Fetch the Recipe.
            var criteria = new SimpleCriteria<int>(accountIdClaim.Value) { Value = id };
            var result = await this.DataManager.ReadAsync(criteria);
            if (result == null || !result.Success || result.Model == null) { return NotFound(); }
            return Ok(result.Model);
        }

        [HttpPost]
        [ResponseType(typeof(Recipe))]
        public async Task<IHttpActionResult> PostRecipe(Recipe recipe)
        {
            // Get the user's account Id.
            var accountIdClaim = this.ClaimsProvider.GetClaim(Utility.AccountIdClaimName);
            if (accountIdClaim == null || String.IsNullOrWhiteSpace(accountIdClaim.Value)) { return Unauthorized(); }

            // Create the Recipe.
            recipe.AccountId = accountIdClaim.Value;
            var result = await this.DataManager.CreateAsync(recipe);
            if (result.Success) { return CreatedAtRoute("DefaultApi", new { id = recipe.Id }, recipe); }
            else { return BadRequest(result.ErrorMessage); }
        }

        [HttpPost]
        [Route("postwithimage")]
        [ResponseType(typeof(Recipe))]
        public async Task<IHttpActionResult> PostRecipe()
        {
            Recipe recipe = await GetRecipeWithImage();
            return await PostRecipe(recipe);
        }

        [HttpPut]
        [ResponseType(typeof(Recipe))]
        public async Task<IHttpActionResult> PutRecipe(Recipe recipe)
        {
            // Get the user's account Id.
            var accountIdClaim = this.ClaimsProvider.GetClaim(Utility.AccountIdClaimName);
            if (accountIdClaim == null || String.IsNullOrWhiteSpace(accountIdClaim.Value)) { return Unauthorized(); }

            // Update the Recipe.
            var criteria = new SimpleCriteria<Recipe>(accountIdClaim.Value) { Value = recipe };
            var result = await this.DataManager.UpdateAsync(criteria);
            if (result.Success) { return Ok(result.Model); }
            else { return BadRequest(result.ErrorMessage); }
        }

        [HttpPut]
        [Route("putwithimage")]
        [ResponseType(typeof(Recipe))]
        public async Task<IHttpActionResult> PutRecipe()
        {
            Recipe recipe = await GetRecipeWithImage();
            return await PutRecipe(recipe);
        }


        private async Task<Recipe> GetRecipeWithImage()
        {
            if (!Request.Content.IsMimeMultipartContent()) { return null; }

            // Instantiate a provider and use it to read the current request content which saves the file to disk.
            var multipartProvider = new MultipartFormDataStreamProvider(Utility.GetTempPath());
            multipartProvider = await Request.Content.ReadAsMultipartAsync(multipartProvider);

            // Get the Recipe object from the form data.
            var recipe = Utility.GetFormData<Recipe>(multipartProvider);
            if (recipe == null || String.IsNullOrWhiteSpace(recipe.ImageFileName)) { return null; }

            // Get a FileInfo object for the temp image file.
            var imageFileInfo = new FileInfo(multipartProvider.FileData.First().LocalFileName);
            string tempFileName = imageFileInfo.FullName;

            // Resize the temp image saving it to the requested file name then delete temp.
            using (var image = CoreUtility.ProcessImage(imageFileInfo, Utility.StandardImageSize))
            {
                imageFileInfo = new FileInfo(Path.Combine(Utility.GetTempPath(), recipe.ImageFileName));
                image.Save(imageFileInfo.FullName);
            }
            File.Delete(tempFileName);

            // Add the image to the BlobStore and then delete it.
            recipe.ImageFileName = await this.BlobStore.AddAsync(imageFileInfo, recipe.ImageFileName.ToLower());
            imageFileInfo.Delete();

            return recipe;
        }


        private readonly IBlobStorage BlobStore;
        private readonly IDataManager<Recipe> DataManager;
    }
}
