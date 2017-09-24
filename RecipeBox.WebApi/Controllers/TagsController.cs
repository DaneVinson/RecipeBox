using RecipeBox.Core;
using RecipeBox.DataContext;
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
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace RecipeBox.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/tags")]
    public class TagsController : BaseController
    {
        public TagsController(IClaimsProvider claimsProvider, IDataManager<Tag> dataManager) : base(claimsProvider)
        {
            this.DataManager = dataManager;
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteTag(int id)
        {
            // Get the user's account Id.
            string accountId = GetAccountId();
            if (accountId == null) { return Unauthorized(); }

            // Create the criteria object and delete.
            var criteria = new SimpleCriteria<int>() { AccountId = accountId, Value = id };
            var result = await this.DataManager.DeleteAsync(criteria);
            if (result.Success) { return NoContent(); }
            else { return NotFound(); }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && this.DataManager != null)
            {
                this.ClaimsProvider = null;
                this.DataManager.Dispose();
                this.DataManager = null;
            }
        }

        [HttpGet]
        public async Task<IEnumerable<Tag>> GetTags()
        {
            // Get the user's account Id.
            string accountId = GetAccountId();
            if (accountId == null) { return new Tag[0]; }
 
            // Get the data.
            var criteria = new SimpleCriteria<EmptyClass>(accountId);
            var result = await this.DataManager.ReadManyAsync(criteria);
 
            // Results
            if (result.Success) { return result.Model; }
            else { return new Tag[0]; }
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(Tag))]
        public async Task<IHttpActionResult> GetTag(int id)
        {
            // Get the user's account Id.
            string accountId = GetAccountId();
            if (accountId == null) { return Unauthorized(); }

            // Fetch the Tag.
            var criteria = new SimpleCriteria<int>(accountId) { Value = id };
            var result = await this.DataManager.ReadAsync(criteria);
            if (result.Success) { return Ok(result.Model); }
            else { return NotFound(); }
        }

        [HttpPost]
        [ResponseType(typeof(Tag))]
        public async Task<IHttpActionResult> PostTag(Tag tag)
        {
            // Get the user's account Id.
            string accountId = GetAccountId();
            if (accountId == null) { return Unauthorized(); }

            // Create the Tag.
            tag.AccountId = accountId;
            var result = await this.DataManager.CreateAsync(tag);
            if (result.Success) { return CreatedAtRoute("DefaultApi", new { id = tag.Id }, tag); }
            else { return BadRequest(result.ErrorMessage); }
        }

        [HttpPut]
        [ResponseType(typeof(Tag))]
        public async Task<IHttpActionResult> PutTag(Tag tag)
        {
            // Get the user's account Id.
            string accountId = GetAccountId();
            if (accountId == null) { return Unauthorized(); }

            // Update the Tag.
            var criteria = new SimpleCriteria<Tag>(accountId) { Value = tag };
            var result = await this.DataManager.UpdateAsync(criteria);
            if (result.Success) { return Ok(result.Model); }
            else { return BadRequest(result.ErrorMessage); }
        }


        private IDataManager<Tag> DataManager { get; set; }
    }
}
