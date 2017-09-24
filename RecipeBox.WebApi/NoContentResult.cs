using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace RecipeBox.WebApi
{
    /// <summary>
    /// IHttpActionResult implementation for a 204 (no content) result.
    /// </summary>
    public class NoContentResult : IHttpActionResult
    {
        private readonly HttpRequestMessage Request;

        /// <summary>
        /// Constructor takes the original request.
        /// </summary>
        /// <param name="request"></param>
        public NoContentResult(HttpRequestMessage request)
        {
            this.Request = request;
        }

        /// <summary>
        /// Method to create an HttpResponseMessage with the correct HttpStatusCode of 204 (no content).
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = this.Request.CreateResponse(HttpStatusCode.NoContent);
            return Task.FromResult(response);
        }
    }
}