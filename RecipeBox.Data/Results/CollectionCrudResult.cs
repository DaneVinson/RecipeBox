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
    /// Basic implementation of the ICrudResult interface with the model as a collection.
    /// </summary>
    public class CollectionCrudResult<T> : BaseCrudResult, ICrudResult<List<T>>
    {
        public CollectionCrudResult() : base()
        {
            this.Model = new List<T>();
        }


        /// <summary>
        /// The model object returned from the CRUD operation if any.
        /// </summary>
        public List<T> Model { get; set; }
    }
}
