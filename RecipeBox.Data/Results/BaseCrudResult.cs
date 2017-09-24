using RecipeBox.Core.Interfaces;
using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Data.Results
{
    /// <summary>
    /// Abstract base class for common memebers of the ICrudResult interface.
    /// </summary>
    public abstract class BaseCrudResult
    {
        public BaseCrudResult()
        {
            this.ErrorMessage = String.Empty;
            this.Messages = new List<string>();
        }


        /// <summary>
        /// Primary failure message for the CRUD operation.
        /// </summary>
        public string ErrorMessage { get; set;  }

        /// <summary>
        /// Collection of result messages if applicable, e.g. multiple errors.
        /// </summary>
        public List<string> Messages { get; set; }

        /// <summary>
        /// Was the CRUD operation a success.
        /// </summary>
        public bool Success { get; set; }
    }
}
