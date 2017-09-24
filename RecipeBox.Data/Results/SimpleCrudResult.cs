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
    /// Basic implementation of the ICrudResult interface.
    /// </summary>
    public class SimpleCrudResult<T> : BaseCrudResult, ICrudResult<T> where T : class
    {
        public SimpleCrudResult() : base() { }


        /// <summary>
        /// The model object returned from the CRUD operation if any.
        /// </summary>
        public T Model { get; set; }
    }
}
